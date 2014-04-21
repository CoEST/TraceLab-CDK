#!/usr/bin/Rscript

# TraceLab Component Library
# Copyright © 2012-2013 SEMERU
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.

# Arguments
#  1 - source dir
#  2 - target dir
#  3 - output file
#  4 - LDA - Min Gibbs Iterations
#  5 - LDA - Max Gibbs Iterations
#  6 - LDA - Max Alpha
#  7 - LDA - Max Eta
#  8 - GA - Max Iterations
#  9 - GA - Run
# 10 - GA - Permutation Rate
# 11 - GA - Population
# 12 - GA - Elitism
# 13 - Seed
args <- commandArgs(TRUE)

if(length(args) != 13 || !file.exists(args[1]) || !file.exists(args[2]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Source: (", file.exists(args[1]), ") ", args[1], "\n",
		"Target: (", file.exists(args[2]), ") ", args[2], "\n",
		"Output: (", file.exists(args[3]), ") ", args[3], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

args.source <- args[1]
args.target <- args[2]
args.output <- args[3]
args.lda.minGibbs <- as.integer(args[4])
args.lda.maxGibbs <- as.integer(args[5])
args.lda.maxAlpha <- as.double(args[6])
args.lda.maxEta   <- as.double(args[7])
args.ga.maxIter <- as.integer(args[8])
args.ga.run     <- as.integer(args[9])
args.ga.pmutate <- as.double(args[10])
args.ga.popsize <- as.integer(args[11])
args.ga.elitism <- as.integer(args[12])
args.seed <- as.integer(args[13])

library(slam)
library(tm)
library(topicmodels)
library(GA)

# build the corpus
x <- DirSource(directory = args.source, ignore.case = TRUE)
leSource <- Corpus(x, readerControl = list(reader = x$DefaultReader, language = "en"))
y <- DirSource(directory = args.target, ignore.case = TRUE)
leTarget <- Corpus(y, readerControl = list(reader = x$DefaultReader, language = "en"))
dtm <- DocumentTermMatrix(c(leSource, leTarget), control = list(stemming = FALSE, stopwords = FALSE))
remove(leSource)
remove(leTarget)
remove(x)
remove(y)

# apply tf-idf
temp   <- weightTfIdf(dtm)
temp$v <- round(temp$v*100)
dtm$v  <- as.integer(temp$v)

# fitness function
fitness_LDA <- function(x=c())
{
    numero_topic <- round(x[1]) # x[1] = number of topics k
    iteration <- round(x[2])    # x[2] = number of gibbs iteration
    pAlpha <- x[3]              # x[3] = Alpha
    pDelta <- x[4]              # x[4] = Beta
    
    # apply LDA to the term-by-document matrix
    ldm <- LDA(dtm, method = "Gibbs", control = list(alpha = pAlpha, delta = pDelta, iter = iteration, seed = args.seed), k = numero_topic)
    pldm <- posterior(ldm)
    names(pldm)

    #compute the topic-by-term matrix    
    names(dtm$dimnames)
    docs <- dtm$dimnames$Docs
    topics <- names(terms(ldm))
    matrix <- pldm$topics
    dimnames(matrix) <- list(docs,topics)
    
    #compute the distance between documents in the topics space
    somiglianze <- 1 - distHellinger(matrix,matrix)
    
    # add names grabbing labels from the corpus
    rownames(somiglianze) <- dtm$dimnames$Docs
    colnames(somiglianze) <- dtm$dimnames$Docs
    
    # computing number of clusters
    clustering <- matrix("", length(rownames(matrix)), 1)
    for (i in 1:length(rownames(matrix)))
    {
        flag <- (matrix[i,] == max(matrix[i,])) # each documents belongs to the cluster with the higher probability
        flag <- which(flag == TRUE)
        if (length(flag) > 1) 
		{
      # if the document has more equivalent topics
      # the cluster is the union of all the equivalent topics
			clustering[i,1] <- ""
			for (t in 1:length(flag))
			{
				clustering[i,1] <- paste(clustering[i,1],flag[t])
			}
		}
        else
            clustering[i,1] <- as.character(flag)
    }
    rownames(clustering) <- rownames(matrix)
    # compute the unique clusters
    clusters <- unique(clustering)
    # get the number of unique clusters
    nCluster <- length(clusters[,1])
    
    # compute the centroids of clusters
    centroids <- simple_triplet_zero_matrix(nrow = 0, ncol = dtm$ncol, mode = "double")
    
    for (i in 1:nCluster)
    {
        flag <- matrix(FALSE,length(row.names(clustering)),1)
        flag[,1] <- as.character(clustering[,1]) == as.character(clusters[i,])
        flag <- which(flag[,1])
        center <- simple_triplet_zero_matrix(nrow = 1, ncol = dtm$ncol, mode = "double")
        count <- 0
        for (row in 1:(length(flag)))
        {
            center <- center + dtm[flag[row],]
            count <- count+1
        }
        center <- center / count
        centroids <- rbind(centroids, center)
    }

	# computing the cohesion of each document
    # it is the maximum distance from a given document Di to the other documents in its cluster
	cohesion <- matrix(0, length(rownames(clustering)), 1)

	for (i in 1:length(row.names(clustering)))
	{
		count <- 0
		for (j in 1:length(row.names(clustering)))
		{
			if (clustering[i,1] == clustering[j,1] && i != j)
			{
				cohesion[i,] <- max(cohesion[i,], sqrt(sum((dtm[i,] - dtm[j,])^2)))
				count <- count + 1
			}
		}
	}
    
    # computing the separation of each document  to the other clusters
    # it's the minimum distance from Di to the centroids of the clusters not containing Di
    separation <- matrix(10000000000000, length(row.names(clustering)), 1)
    
    for (i in 1:length(row.names(clustering)))
    {
		count <- 0
		for (j in 1:length(clusters))
		{
			if (clustering[i,1] != clusters[j,1])
			{
				separation[i,] <- min(separation[i,], sqrt(sum((dtm[i,] - centroids[j,])^2)))
				count <- count + 1
			}
		}
    }
    
    # compute the silhouette coefficient for all documents
    maxi <- matrix(0, nCluster, 1)
    for (i in 1:length(rownames(clustering)))
	{
		maxi[i] <- max(separation[i,1], cohesion[i,1])
	}
    z <- mean((separation-cohesion)/maxi)
    # print fitness value
    # print(paste(z, x[1], x[2], x[3], x[4]))
    return (z) # for GA package
}

# running GA
res <- ga(type = "real-valued",
	fitness = fitness_LDA,
	elitism = args.ga.elitism,
	min = c(2, args.lda.minGibbs, 0.1, 0.1),
	max = c(dtm$nrow, args.lda.maxGibbs, args.lda.maxAlpha, args.lda.maxEta),
	pmutation = args.ga.pmutate,
	maxiter = args.ga.maxIter,
	run = args.ga.run,
	popSize = args.ga.popsize
)

# save values
write.table(slot(res, "solution"), args.output, sep="\t", quote=FALSE, col.names=FALSE, row.names=FALSE)