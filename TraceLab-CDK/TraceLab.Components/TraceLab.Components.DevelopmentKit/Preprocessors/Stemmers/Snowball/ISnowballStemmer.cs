/*
 *  Port of Snowball stemmers on C#
 *  Original stemmers can be found on http://snowball.tartarus.org
 *  Licence still BSD: http://snowball.tartarus.org/license.php
 *  
 *  Most of stemmers are ported from Java by Iveonik Systems ltd. (www.iveonik.com)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball
{
    /// <summary>
    /// Snowball stemmer interface
    /// </summary>
    public interface ISnowballStemmer
    {
        /// <summary>
        /// Stems terms in a string
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Stemmed terms</returns>
        string Stem(string s);
    }
}
