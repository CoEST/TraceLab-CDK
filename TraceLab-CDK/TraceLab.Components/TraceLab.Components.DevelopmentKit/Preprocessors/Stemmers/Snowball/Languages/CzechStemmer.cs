/*
 *  Port of Snowball stemmers on C#
 *  Original stemmers can be found on http://snowball.tartarus.org
 *  Licence still BSD: http://snowball.tartarus.org/license.php
 *  
 *  Most of stemmers are ported from Java by Iveonik Systems ltd. (www.iveonik.com)
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball.Languages
{
    /// <summary>
    /// Czech language Snowball implementation
    /// </summary>
    public class CzechStemmer : StemmerOperations, ISnowballStemmer
    {
        /// <summary>
        /// Stems terms in the input string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Stemmed terms</returns>
        public string Stem(string input)
        {
            setCurrent(input.ToLowerInvariant());
            // stemming...
            //removes case endings from nouns and adjectives
            removeCase();
            //removes possesive endings from names -ov- and -in-
            removePossessives();
            //removes comparative endings
            removeComparative();
            //removes diminutive endings
            removeDiminutive();
            //removes augmentatives endings
            removeAugmentative();
            //removes derivational sufixes from nouns
            removeDerivational();
            //result = sb.toString();
            return getCurrent(); //sb.ToString();
        }

    }


}
