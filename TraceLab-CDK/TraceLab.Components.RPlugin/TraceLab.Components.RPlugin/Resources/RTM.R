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
#  4 - links file
#  5 - output file
#  6 - number of topics
#  7 - number of iterations
#  8 - alpha
#  9 - eta
# 10 - beta0
# 11 - beta1
# 12 - seed
args <- commandArgs(TRUE)

if(length(args) != 12 || !file.exists(args[1]) || !file.exists(args[2]) || !file.exists(args[3]) || !file.exists(args[4]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Matrix: (", file.exists(args[1]), ") ", args[1], "\n",
		"Vocab:  (", file.exists(args[2]), ") ", args[2], "\n",
		"Edges:  (", file.exists(args[3]), ") ", args[3], "\n",
		"Links:  (", file.exists(args[4]), ") ", args[4], "\n",
		"Output: (", file.exists(args[5]), ") ", args[5], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

library(lda)
docs     <- dget(args[1])
vocab    <- dget(args[2])
edges    <- read.table(args[3])
links    <- dget(args[4])
outFile  <- args[5]
topicCnt <- as.integer(args[6])
iterCnt  <- as.integer(args[7])
alpha    <- as.double(args[8])
eta      <- as.double(args[9])
beta0    <- as.double(args[10])
beta1    <- as.double(args[11])

set.seed(as.integer(args[12]))

#Fit an RTM model
rtm.model <- rtm.collapsed.gibbs.sampler(docs, links, topicCnt, vocab, iterCnt, alpha, eta, beta0)

#Compute link probability
rtm.similarity <- predictive.link.probability(edges, rtm.model$document_sums, alpha, beta1)

#Save results
dput(rtm.similarity, outFile)