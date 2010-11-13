using System;
using System.IO;

using AlmWitt.Web.ResourceManagement.ContentFiltering;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
    [TestFixture]
    public class TestResourceCollection
    {
        [Test]
        public void LastModifiedReturnsGreatestValueFromItems()
        {
            DateTime mostRecent = DateTime.Now;

            ResourceCollection resources = new ResourceCollection();
            resources.Add(CreateResource(mostRecent.AddDays(-1)));
            resources.Add(CreateResource(mostRecent));
            resources.Add(CreateResource(mostRecent.AddDays(-30)));

            Assert.That(resources.LastModified, Is.EqualTo(mostRecent));
        }

        [Test]
        public void ConsolidateEqualToSumOfParts()
        {
            ResourceCollection resources = new ResourceCollection();
            StringWriter expectedWriter = new StringWriter();
            AddResourceContent(resources, "my content", expectedWriter);
            AddResourceContent(resources, "my other content", expectedWriter);

            StringWriter actualWriter = new StringWriter();
            resources.ConsolidateContentTo(actualWriter);

            Assert.That(actualWriter.ToString(), Is.EqualTo(expectedWriter.ToString()));
        }

        [Test]
        public void ConsolidatePlacesSeparatorStringBetweenParts()
        {
            string separator = Environment.NewLine;

            ResourceCollection resources = new ResourceCollection();
            StringWriter expectedWriter = new StringWriter();
            AddResourceContent(resources, "my content", expectedWriter);
            expectedWriter.Write(separator);
            AddResourceContent(resources, "my other content", expectedWriter);

            StringWriter actualWriter = new StringWriter();
            resources.ConsolidateContentTo(actualWriter, separator);

            Assert.That(actualWriter.ToString(), Is.EqualTo(expectedWriter.ToString()));
        }

        [Test]
        public void ConsolidateFiltersContent()
        {
            string contentA = "a a a";
            string contentB = "b b b";
            string separator = Environment.NewLine;

            ResourceCollection resources = new ResourceCollection();
            resources.Add(new StubResource(contentA));
            resources.Add(new StubResource(contentB));

            StringWriter actualWriter = new StringWriter();
            resources.ConsolidateContentTo(actualWriter, g => new RemoveSpacesFilter(), separator);

            Assert.That(actualWriter.ToString(), Is.EqualTo(String.Format("aaa{0}bbb", Environment.NewLine)));
        }

        private static void AddResourceContent(ResourceCollection resources, string content, StringWriter writer)
        {
            resources.Add(new StubResource(content));
            writer.Write(content);
        }

        private static IResource CreateResource(DateTime lastModified)
        {
            StubResource resource = new StubResource(String.Empty);
            resource.LastModified = lastModified;

            return resource;
        }

        private class RemoveSpacesFilter : IContentFilter
        {
            public string FilterContent(string content)
            {
                return content.Replace(" ", "");
            }
        }
    }
}