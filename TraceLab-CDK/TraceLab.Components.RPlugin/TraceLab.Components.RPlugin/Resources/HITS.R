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
# 2 - output authorities file
# 3 - output hubs file
# 4 - epsilon

args <- commandArgs(TRUE)

if(length(args) != 4 || !file.exists(args[1]))
{
	cat("Debug:\n",
		"Args:   ", length(args), "\n",
		"Matrix: (", file.exists(args[1]), ") ", args[1], "\n",
		"Output (authories): (", file.exists(args[2]), ") ", args[2], "\n",
		"Output (hubs): (", file.exists(args[3]), ") ", args[3], "\n",
		"Epsilon: ", args[4], "\n",
		sep=""
	)
	stop("Invalid arguments.")
}

inputFileMatrix = args[1]
outputFileRanksAuthorities = args[2]
outputFileRanksHubs = args[3]
epsilonThreshold = as.double(args[4])

#we need to make sure there is no extra space at the end of each line in the file
#if there is a space, the matrix will not be n x n and we get an error
am=read.table(inputFileMatrix,header=FALSE,sep=" ")

n=dim(am)
n=n[1]

#set the a0 and h0 column vector to 1
a0=matrix(data=1,nrow=n,ncol=1)
h0=matrix(data=1,nrow=n,ncol=1)

a1=as.matrix(t(am)) %*% as.matrix(am) %*% a0
h1=as.matrix(am) %*% as.matrix(t(am)) %*% h0

a1=a1 / norm(as.vector(a1),type="2")
h1=h1 / norm(as.vector(h1),type="2")

step=0
while (norm(a1-a0,type="O")>epsilonThreshold)
{
  a0=a1
  h0=h1
  
  a1=as.matrix(t(am)) %*% as.matrix(am) %*% a0
  h1=as.matrix(am) %*% as.matrix(t(am)) %*% h0

  a1=a1 / norm(as.vector(a1),type="2")
  h1=h1 / norm(as.vector(h1),type="2")
  
  step=step+1  
}

ranksAuthorities=a1
ranksHubs=h1

#print(ranksAuthorities)
#print(ranksHubs)
#print(paste("Norm",norm(a1-a0,type="O")))
#print(paste("Norm",norm(h1-h0,type="O")))
#print(paste("Step",step))

write(format(ranksAuthorities,digits=22),ncolumns=1,outputFileRanksAuthorities)
write(format(ranksHubs,digits=22),ncolumns=1,outputFileRanksHubs)
#print(paste("File with ranksAuthorities was saved at:",outputFileRanksAuthorities))
#print(paste("File with ranksHubs was saved at:",outputFileRanksHubs))
