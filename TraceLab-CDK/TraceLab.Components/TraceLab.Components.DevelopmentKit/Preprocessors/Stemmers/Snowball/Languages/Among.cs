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

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball.Languages
{

    internal class Among
    {

        public readonly int s_size; /* search string */
        public readonly char[] s; /* search string */
        public readonly int substring_i; /* index to longest matching substring */
        public readonly int result; /* result of the lookup */
        public delegate bool boolDel();
        public readonly boolDel method; /* method to use if substring matches */

        public Among(string s, int substring_i, int result, boolDel linkMethod)
        {
            this.s_size = s.Length;
            this.s = s.ToCharArray();
            this.substring_i = substring_i;
            this.result = result;
            this.method = linkMethod;
        }
    }
}
