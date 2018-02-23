﻿using System.Linq;
using FluentAssertions.Collections;
using Tupi.Indexing;
using Tupi.Querying;
using Tupi.Querying.Queries;
using Xunit;

namespace Tupi.Tests.Functional
{
    public class Boolean
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
            new[] {"elemar", "inverted"},
            new[] {0, 3}
        )]

        public void All(string[] documents, string[] terms, int[] expectedResults)
        {
            var index = new StringIndexer().CreateIndex(documents);
            var seacher = new Searcher(index);

            var results = seacher.Search(Query.All(
                terms.Select(t => TermQuery.From(t, DefaultAnalyzer.Instance)))
                ).ToArray();

            Assert.Equal(expectedResults, results);
        }

        [Fact]
        public void Or_Term_And()
        {
            var documents = new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is cool to use csharp.",
                "This is a simple string document created by Elemar",
                "This string will be indexed in an inverted index generated by Elemar"
            };

            var index = new StringIndexer().CreateIndex(documents);
            var seacher = new Searcher(index);

            var query = Query.Or(
                Query.Term("csharp"),
                Query.And(
                    Query.Term("Elemar"),
                    Query.Term("inverted")
                )
            );

            var results = seacher.Search(query);

            Assert.Equal(new[] { 0, 1, 3 }, results.OrderBy(m => m));
        }

        [Fact]
        public void Not_And()
        {
            var documents = new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is cool to use csharp.",
                "This is a simple string document created by Elemar",
                "This string will be indexed in an inverted index generated by Elemar"
            };

            var index = new StringIndexer().CreateIndex(documents);
            var seacher = new Searcher(index);

            var query = Query.Not(
                Query.And(
                    Query.Term("Elemar"),
                    Query.Term("inverted")
                )
            );

            var results = seacher.Search(query);

            Assert.Equal(new[] { 1, 2 }, results.OrderBy(m => m));
        }

        [Fact]
        void Distance()
        {
            var documents = new[]
            {
                "Elemar is learning how to create an inverted index",
                "It is inverted but it is not an index.",
                "This is a simple string document created by Elemar",
                "This string will be indexed in an inverted index generated by Elemar"
            };

            var index = new StringIndexer().CreateIndex(documents);
            var seacher = new Searcher(index);

            var query = Query.Distance(
                DefaultAnalyzer.Instance.AnalyzeOnlyTheFirstToken("inverted"),
                DefaultAnalyzer.Instance.AnalyzeOnlyTheFirstToken("index"),
                1
            );

            var results = seacher.Search(query);

            Assert.Equal(new[] { 0, 3 }, results.OrderBy(m => m));
        }
    }
}
