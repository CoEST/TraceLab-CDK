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
#  4 - LDA - Num topics
#  5 - LDA - Num Gibbs iterations
#  6 - LDA - Alpha
#  7 - LDA - Beta
#  8 - LDA - Seed
args <- commandArgs(TRUE)

if(length(args) != 8 || !file.exists(args[1]) || !file.exists(args[2]))
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
args.lda.topics <- as.integer(args[4])
args.lda.gibbs <- as.integer(args[5])
args.lda.alpha <- as.double(args[6])
args.lda.beta   <- as.double(args[7])
args.lda.seed <- as.integer(args[8])

library(slam)
library(tm)
library(topicmodels)

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
remove(temp)

# apply LDA to the term-by-document matrix
ldm <- LDA(dtm, method = "Gibbs", control = list(alpha = args.lda.alpha, delta = args.lda.beta, iter = args.lda.gibbs, seed = args.lda.seed), k = args.lda.topics)
pldm <- posterior(ldm)
#names(pldm)

# compute the topic-by-term matrix    
#names(dtm$dimnames)
docs <- dtm$dimnames$Docs
topics <- names(terms(ldm))
matrix <- pldm$topics
dimnames(matrix) <- list(docs,topics)
    
# compute the distance between documents in the topics space
somiglianze <- 1 - distHellinger(matrix,matrix)
    
# add names grabbing labels from the corpus
rownames(somiglianze) <- dtm$dimnames$Docs
colnames(somiglianze) <- dtm$dimnames$Docs

# load the filenames of UC and  CC
vetUC = list.files( args.source )
vetCC = list.files( args.target )
doc <- colnames( somiglianze )

# create a submatrix where saving the similarities between UC and CC only
matrix <- matrix ( -1, length(vetCC), length(vetUC) )

# add documents names to the matrix
rownames(matrix) <- vetCC
colnames(matrix) <- vetUC

# exctract the similarities bewteen UC and CC only (ingnoring silimarities between UC-UC and CC-CC)
for ( i in 1:length(vetCC) )
{
	for( j in 1:length(vetUC) )
	{
		index_i <- which( doc == vetCC[i] ) # ritorna la posizione di doc uguale a vetCC[i]
		index_j <- which( doc == vetUC[j] ) # ritorna la posizione di doc uguale a vetUC[i]
		matrix[i,j] <- somiglianze[index_i, index_j]
	}	
}

# create the ranked list
rlist <- matrix ( -1, length(vetCC) * length(vetUC), 3 )
# add column names to the ranked list
colnames(rlist) <- c("UC", "CC", "Similarity")
n = 1;

# insert all pair of UC-CC in the ranked list
for ( i in 1:length(vetUC) )
{
	for( j in 1:length(vetCC) )
	{  	
		rlist[n, 1] <- vetUC[i]
		rlist[n, 2] <- vetCC[j]  	
		rlist[n, 3] <- matrix[j, i]
		n = n + 1;
	}
}

# order the ranked list in descending order of similarities
rlist <- rlist [ order( as.real(rlist[,3]), decreasing=TRUE) , ]

# save ranked list
write.table(rlist, args.output, sep="\t", quote=FALSE, col.names=FALSE, row.names=FALSE)