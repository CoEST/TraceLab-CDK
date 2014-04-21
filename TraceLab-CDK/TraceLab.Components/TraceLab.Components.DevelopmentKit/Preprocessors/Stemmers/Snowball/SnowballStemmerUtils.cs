// TraceLab Component Library
// Copyright © 2012-2013 SEMERU
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball.Languages;

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball
{
    /// <summary>
    /// Provides additional functionality for the Snowball namespace
    /// </summary>
    public static class SnowballStemmerUtils
    {
        /// <summary>
        /// Returns a stemmer object that implements the Snowball stemmer algorithm in the given language.
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Snowball stemmer</returns>
        public static ISnowballStemmer GetStemmer(SnowballStemmerEnum language)
        {
            switch (language)
            {
                case SnowballStemmerEnum.Czech:
                    return new CzechStemmer();

                case SnowballStemmerEnum.Danish:
                    return new DanishStemmer();

                case SnowballStemmerEnum.Dutch:
                    return new DutchStemmer();

                case SnowballStemmerEnum.Finnish:
                    return new FinnishStemmer();

                case SnowballStemmerEnum.French:
                    return new FrenchStemmer();

                case SnowballStemmerEnum.German:
                    return new GermanStemmer();

                case SnowballStemmerEnum.Hungarian:
                    return new HungarianStemmer();

                case SnowballStemmerEnum.Italian:
                    return new ItalianStemmer();

                case SnowballStemmerEnum.Norwegian:
                    return new NorwegianStemmer();

                case SnowballStemmerEnum.Portuguese:
                    return new PortugalStemmer();

                case SnowballStemmerEnum.Romanian:
                    return new RomanianStemmer();

                case SnowballStemmerEnum.Russian:
                    return new RussianStemmer();

                case SnowballStemmerEnum.Spanish:
                    return new SpanishStemmer();

                case SnowballStemmerEnum.English:
                case SnowballStemmerEnum.Default:
                default:
                    return new EnglishStemmer();
            }
        }
    }
}
