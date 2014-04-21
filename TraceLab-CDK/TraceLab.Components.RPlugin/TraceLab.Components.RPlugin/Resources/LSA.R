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

# 1 - artifacts directory
# 2 - source artifacts table
# 3 - target artifacts table
# 4 - output file
# 5 - number of dimensions

args <- commandArgs(TRUE)

if(length(args) != 5 || !file.exists(args[1]) || !file.exists(args[2]) || !file.exists(args[3]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Corpus: (", file.exists(args[1]), ") ", args[1], "\n",
		"Source: (", file.exists(args[2]), ") ", args[2], "\n",
		"Target: (", file.exists(args[3]), ") ", args[3], "\n",
		"Output: ", args[4], "\n",
		"Dims:   ", args[5], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

library(lsa)

# tf function
real_tf <- function(m)
{
	return (sweep(m, MARGIN=2, apply(m, 2, max), "/"))
}

#idf function
real_idf <- function(m)
{
	df = rowSums(lw_bintf(m), na.rm=TRUE)
	return (log(ncol(m)/df))
}

#load corpus
lsa.documents <- textmatrix(args[1], minWordLength=1, minDocFreq=0)

# compute tf-idf
lsa.weighted_documents <- real_tf(lsa.documents) * real_idf(lsa.documents)

# compute svd
lsa.nspace <- lsa(lsa.weighted_documents, dims = as.integer(args[5]))
lsa.matrix <- diag(lsa.nspace$sk) %*% t(lsa.nspace$dk)

# compute similarities
lsa.sourceIDs <- scan(args[2], what = character())
lsa.targetIDs <- scan(args[3], what = character())
sourceMatrix <- lsa.matrix[,lsa.sourceIDs]
targetMatrix <- lsa.matrix[,lsa.targetIDs]

cycleM2 <- function(x, matrix)
{
	apply(targetMatrix,2,cosine,x)
}

sims <- apply(sourceMatrix,2,cycleM2)
write.table(sims, args[4], quote=FALSE)