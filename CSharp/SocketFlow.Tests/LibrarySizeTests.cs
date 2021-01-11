using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SocketFlow.Client;
using SocketFlow.Server;

namespace SocketFlow.Tests
{
    [TestFixture]
    public class LibrarySizeTests
    {
        private const int ClientSize = 20 * 1024;
        private const int CommonLibrarySize = 20 * 1024;
        private const int ServerSize = 100 * 1024;

        [Test]
        public void Client()
        {
            var location = typeof(FlowClient).Assembly.Location;
            new FileInfo(location).Length.Should().BeLessThan(ClientSize);
        }

        [Test]
        public void Server()
        {
            var location = typeof(FlowServer).Assembly.Location;
            new FileInfo(location).Length.Should().BeLessThan(ServerSize);
        }

        [Test]
        public void CommonLibrary()
        {
            var location = typeof(NetworkStreamExtensions).Assembly.Location;
            new FileInfo(location).Length.Should().BeLessThan(CommonLibrarySize);
        }
    }
}
