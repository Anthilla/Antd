using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nancy.ViewEngines.SuperSimpleViewEngine;

namespace Antd2 {
    internal sealed class AntdViewEngine :
        ISuperSimpleViewEngineMatcher {
        /// <summary>
        ///   Compiled Regex for translation substitutions.
        /// </summary>
        private static readonly Regex TranslationSubstitutionsRegEx;

        static AntdViewEngine() {
            // This regex will match strings like:
            // @Translate.Hello_World
            // @Translate.FooBarBaz;
            TranslationSubstitutionsRegEx =
                new Regex(
                    @"@Translate\.(?<TranslationKey>[a-zA-Z0-9-_]+);?",
                    RegexOptions.Compiled);
        }

        public string Invoke(string content, dynamic model, IViewEngineHost host) {
            return TranslationSubstitutionsRegEx.Replace(
                content,
                m => {
                    // A match was found!

                    // Get the translation 'key'.
                    var translationKey = m.Groups["TranslationKey"].Value;

                    // Load the appropriate translation.  This could farm off to
                    // a ResourceManager for example.  The below implementation
                    // obviously isn't very useful and is just illustrative. :)
                    var translationResult = translationKey == "Hello_World" ? "Hello World!" : translationKey;

                    return translationResult;
                });
        }
    }
}