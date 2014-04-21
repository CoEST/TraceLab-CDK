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

# 1 - input matrix file
# 2 - output ranks file
# 3 - beta
# 4 - epsilon

args <- commandArgs(TRUE)

if(length(args) != 4 || !file.exists(args[1]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Matrix: (", file.exists(args[1]), ") ", args[1], "\n",
		"Output: (", file.exists(args[2]), ") ", args[2], "\n",
		"Beta: ", args[3], "\n",
		"Epsilon: ", args[4], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

inputFileMatrix = args[1]
outputFileRanks = args[2]
beta = as.double(args[3])
epsilonThreshold = as.double(args[4])

#we need to make sure there is no extra space at the end of each line in the file
#if there is a space, the matrix will not be n x n and we get an error
tpm=read.table(inputFileMatrix,header=FALSE,sep=" ")

n=dim(tpm)
n=n[1]
#cat(n)

tpm=t(tpm)

#set the r0 column vector to 1/n
r0=matrix(data=1,nrow=n,ncol=1)/n

# M = 0.85*M + 0.15*r; where r is a n x n matrix of values 1/n
tpm=beta*tpm+(1-beta)*matrix(data=1,nrow=n,ncol=n)/n

#the %*% is the matrix multiplicator operator
r1=tpm %*% r0

step=0
while (norm(r1-r0,type="O")>epsilonThreshold)
{
  r0=r1
  r1=tpm %*% r0
  step=step+1;
}

ranks=r1
#print(ranks)
#print(paste("Norm",norm(r1-r0,type="O")))
#print(paste("Step",step))

write(format(ranks,digits=22),ncolumns=1,outputFileRanks)
#print(paste("File with ranks was saved at:",outputFileRanks))
