#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Tracks
{
    [TestFixture]
    internal class TrackTests
    {
        private void TestMktImportExport(int trackGroupId, int trackId)
        {
            var game = File.GetGame(Region.US);

            var track1 = game.TrackGroups[trackGroupId][trackId];
            var track2 = game.TrackGroups[0][0];
            var filePath = File.GetOutputPath($"track_{trackGroupId}_{trackId}.mkt");

            track1.Export(filePath, game);
            track2.Import(filePath, game);

            Assert.AreEqual(track1.Map.GetBytes(), track2.Map.GetBytes());
            Assert.AreEqual(game.Themes.GetThemeId(track1.Theme), game.Themes.GetThemeId(track2.Theme));
        }

        [Test]
        public void TestMktImportExport1()
        {
            TestMktImportExport(0, 0);
        }

        [Test]
        public void TestMktImportExport2()
        {
            TestMktImportExport(0, 1);
        }

        [Test]
        public void TestMktImportExport3()
        {
            TestMktImportExport(0, 2);
        }

        [Test]
        public void TestMktImportExport4()
        {
            TestMktImportExport(0, 3);
        }

        [Test]
        public void TestMktImportExport5()
        {
            TestMktImportExport(0, 4);
        }

        [Test]
        public void TestMktImportExport6()
        {
            TestMktImportExport(1, 0);
        }

        [Test]
        public void TestMktImportExport7()
        {
            TestMktImportExport(1, 1);
        }

        [Test]
        public void TestMktImportExport8()
        {
            TestMktImportExport(1, 2);
        }

        [Test]
        public void TestMktImportExport9()
        {
            TestMktImportExport(1, 3);
        }

        [Test]
        public void TestMktImportExport10()
        {
            TestMktImportExport(1, 4);
        }

        [Test]
        public void TestMktImportExport11()
        {
            TestMktImportExport(2, 0);
        }

        [Test]
        public void TestMktImportExport12()
        {
            TestMktImportExport(2, 1);
        }

        [Test]
        public void TestMktImportExport13()
        {
            TestMktImportExport(2, 2);
        }

        [Test]
        public void TestMktImportExport14()
        {
            TestMktImportExport(2, 3);
        }

        [Test]
        public void TestMktImportExport15()
        {
            TestMktImportExport(2, 4);
        }

        [Test]
        public void TestMktImportExport16()
        {
            TestMktImportExport(3, 0);
        }

        [Test]
        public void TestMktImportExport17()
        {
            TestMktImportExport(3, 1);
        }

        [Test]
        public void TestMktImportExport18()
        {
            TestMktImportExport(3, 2);
        }

        [Test]
        public void TestMktImportExport19()
        {
            TestMktImportExport(3, 3);
        }

        [Test]
        public void TestMktImportExport20()
        {
            TestMktImportExport(3, 4);
        }

        [Test]
        public void TestMktImportExport21()
        {
            TestMktImportExport(4, 0);
        }

        [Test]
        public void TestMktImportExport22()
        {
            TestMktImportExport(4, 1);
        }

        [Test]
        public void TestMktImportExport23()
        {
            TestMktImportExport(4, 2);
        }

        [Test]
        public void TestMktImportExport24()
        {
            TestMktImportExport(4, 3);
        }
    }
}
