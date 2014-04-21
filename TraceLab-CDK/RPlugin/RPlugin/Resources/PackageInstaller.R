#!/usr/bin/Rscript

# RPlugin - A framework for running R scripts in .NET
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
# along with this program.  If not, see <http:#www.gnu.org/licenses/>.

# Installs necessary R packages
# Takes in package names as arguments

#  1  - (0) check packages
#     - (1) install packages
# ... - package names
args <- commandArgs(TRUE)

# package checking function
is.installed <- function(mypkg) is.element(mypkg, installed.packages()[,1])

if (args[1] == 1)
{
	cat("Installing missing packages...\n")
}

# check arguments
for (i in 2:length(args))
{
	if (!is.installed(args[i]))
	{
		if (args[1] == 1)
		{
			cat(paste("Installing", args[i], "...\n"))
			install.packages(args[i], repos="http://lib.stat.cmu.edu/R/CRAN")
		}
		else
		{
			cat(paste(args[i], " ", sep=""))
		}
	}
	else
	{
		if (args[1] == 1)
		{
			cat(paste(args[i], "found!\n"))
		}
	}
}