using System;

namespace NRadio.Helpers
{
    public static class SearchHelper
    {
        private static dynamic emptySearchPage;

        public static dynamic SearchPage 
        {
            get => emptySearchPage;
        }

        public static void Initialize(Type searchPage)
        {
            emptySearchPage = searchPage;
        }
    }
}