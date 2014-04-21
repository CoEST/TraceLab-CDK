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

# 1 - input file
# 2 - output file
args <- commandArgs(TRUE)

if(length(args) != 2 || !file.exists(args[1]))
{
	cat ("Debug: (", length(args), ")\n");
	cat ("Input File: ", args[1], " ", file.exists(args[1]), "\n");
	cat ("Output File: ", args[2], " ", file.exists(args[2]), "\n");
	stop("Invalid arguments.")
}

library(psych)
library(GPArotation)
data  <- read.table(args[1])
oFile <- args[2]

pca    <- principal(data, nfactors = ncol(data))
pc     <- colSums(pca$loadings^2) / ncol(data)
max_pc <- apply(pca$loadings, 1, which.max)
max_pc_vals <- pc[apply(pca$loadings, 1, which.max)]

#Save model
write.csv(cbind(max_pc, max_pc_vals), oFile)