﻿using System.IO;
using System.Linq;
using Tupi.Indexing;
using Tupi.Querying;
using Xunit;

namespace Tupi.Tests.Functional
{
    public class Basic
    {

        [Theory]
        [InlineData(
            new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is cool to use csharp.",
                "This is a simple string document created by Elemar",
                "This string will be indexed in an inverted index generated by Elemar"
            },
            "elemar",
            new[] {0, 2, 3}
        )]
        [InlineData(
            new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is cool to use csharp.",
            },
            "visual",
            new int[]{}
        )]

        public void SearchBySingleTerm(
            string[] documents,
            string term,
            int[] expectedResults
        )
        {
            var index = new Indexer().CreateIndex(documents);
            var searcher = new Searcher(index);
            var results = searcher.Search(term);
            Assert.Equal(expectedResults, results);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is cool to use csharp.",
                "This is a simple string document created by Elemar",
                "This string will be indexed in an inverted index generated by Elemar"
            },
            "Elemar inverted",
            new[] { 0, 3 }
        )]
        public void SearchByQuery(
            string[] documents,
            string query,
            int[] expectedResults
        )
        {
            string[] terms;
            using (var reader = new StringReader(query))
            {
                terms = new TokenSource(reader)
                    .ReadAll(DefaultAnalyzer.Instance.Process)
                    .ToArray();
            }

            var index = new Indexer().CreateIndex(documents);
            var searcher = new Searcher(index);
            var results = searcher.Search(terms);
            Assert.Equal(expectedResults, results);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "Human cannibalism is the act or practice of humans eating the flesh or internal organs of other human beings. ",
                "There are cannibals in some primitive communities.",
                "In marketing strategy, cannibalization refers to a reduction in sales volume, sales revenue,... ",
            },
            "Cannibalization",
            new[] { 0, 1, 2 }
        )]
        public void SearchByQuery_TestingPorterStemming(
            string[] documents,
            string query,
            int[] expectedResults
        )
        {
            var index = new Indexer().CreateIndex(documents);
            var searcher = new Searcher(index);
            var results = searcher.Search(query, DefaultAnalyzer.Instance);
            Assert.Equal(expectedResults, results);
        }
    }
}
