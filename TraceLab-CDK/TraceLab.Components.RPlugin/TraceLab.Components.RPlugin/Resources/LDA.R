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

#  1 - matrix file
#  2 - vocab file
#  3 - edges file
#  4 - output file
#  5 - number of topics
#  6 - number of iterations
#  7 - alpha
#  8 - eta
#  9 - beta1
# 10 - seed
args <- commandArgs(TRUE)

if(length(args) != 10 || !file.exists(args[1]) || !file.exists(args[2]) || !file.exists(args[3]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Matrix: (", file.exists(args[1]), ") ", args[1], "\n",
		"Vocab:  (", file.exists(args[2]), ") ", args[2], "\n",
		"Edges:  (", file.exists(args[3]), ") ", args[3], "\n",
		"Output: (", file.exists(args[5]), ") ", args[5], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

library(lda)
docs     <- dget(args[1])
vocab    <- dget(args[2])
edges    <- read.table(args[3])
outFile  <- args[4]
topicCnt <- as.integer(args[5])
iterCnt  <- as.integer(args[6])
alpha    <- as.double(args[7])
eta      <- as.double(args[8])
beta1    <- as.double(args[9])

set.seed(as.integer(args[10]))

#Fit an LDA model
lda.model <- lda.collapsed.gibbs.sampler(docs, topicCnt, vocab, iterCnt, alpha, eta)

#Compute link probability
lda.similarity <- predictive.link.probability(edges, lda.model$document_sums, alpha, beta1)

#Save results
dput(lda.similarity, outFile)