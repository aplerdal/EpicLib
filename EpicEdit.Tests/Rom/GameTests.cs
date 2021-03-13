﻿#region GPL statement
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
using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom
{
    [TestFixture]
    internal class GameTests
    {
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _game = File.GetGame(Region.US);
        }

        [Test]
        public void TestTrackGroupCount()
        {
            Assert.AreEqual(5, _game.TrackGroups.Count);
        }

        [Test]
        public void TestTrackSizes()
        {
            foreach (TrackGroup trackGroup in _game.TrackGroups)
            {
                foreach (Track track in trackGroup)
                {
                    Assert.AreEqual(128, track.Map.Width);
                    Assert.AreEqual(128, track.Map.Height);
                }
            }
        }

        [Test]
        public void TestTrackGroupName1()
        {
            Assert.AreEqual("Mushroom Cup", _game.TrackGroups[0].Name);
        }

        [Test]
        public void TestTrackGroupName2()
        {
            Assert.AreEqual("Flower Cup", _game.TrackGroups[1].Name);
        }

        [Test]
        public void TestTrackGroupName3()
        {
            Assert.AreEqual("Star Cup", _game.TrackGroups[2].Name);
        }

        [Test]
        public void TestTrackGroupName4()
        {
            Assert.AreEqual("Special Cup", _game.TrackGroups[3].Name);
        }

        [Test]
        public void TestTrackGroupName5()
        {
            Assert.AreEqual("Battle Course ", _game.TrackGroups[4].Name);
        }

        [Test]
        public void TestTrackName1()
        {
            Assert.AreEqual("Mario Circuit 1", _game.TrackGroups[0][0].Name);
        }

        [Test]
        public void TestTrackName2()
        {
            Assert.AreEqual("Donut Plains 1", _game.TrackGroups[0][1].Name);
        }

        [Test]
        public void TestTrackName3()
        {
            Assert.AreEqual("Ghost Valley 1", _game.TrackGroups[0][2].Name);
        }

        [Test]
        public void TestTrackName20()
        {
            Assert.AreEqual("Rainbow Road ", _game.TrackGroups[3][4].Name);
        }

        [Test]
        public void TestTrackTheme1()
        {
            Assert.AreEqual("Mario Circuit ", _game.TrackGroups[0][0].Theme.Name);
        }

        [Test]
        public void TestTrackTheme2()
        {
            Assert.AreEqual("Donut Plains ", _game.TrackGroups[0][1].Theme.Name);
        }

        [Test]
        public void TestTrackTheme3()
        {
            Assert.AreEqual("Ghost Valley ", _game.TrackGroups[0][2].Theme.Name);
        }

        [Test]
        public void TestGPTrackReorderReferences()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Track[] tracks =
            {
                game.TrackGroups[0][0],
                game.TrackGroups[0][1],
                game.TrackGroups[0][2],
                game.TrackGroups[0][3],
                game.TrackGroups[0][4],
                game.TrackGroups[1][0],
                game.TrackGroups[1][1],
                game.TrackGroups[1][2],
                game.TrackGroups[1][3],
                game.TrackGroups[1][4]
            };

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            Assert.AreEqual(tracks[0], game.TrackGroups[0][0]);
            Assert.AreEqual(tracks[2], game.TrackGroups[0][1]);
            Assert.AreEqual(tracks[3], game.TrackGroups[0][2]);
            Assert.AreEqual(tracks[4], game.TrackGroups[0][3]);
            Assert.AreEqual(tracks[5], game.TrackGroups[0][4]);
            Assert.AreEqual(tracks[6], game.TrackGroups[1][0]);
            Assert.AreEqual(tracks[7], game.TrackGroups[1][1]);
            Assert.AreEqual(tracks[8], game.TrackGroups[1][2]);
            Assert.AreEqual(tracks[1], game.TrackGroups[1][3]);
            Assert.AreEqual(tracks[9], game.TrackGroups[1][4]);
        }

        [Test]
        public void TestGPTrackReorderNames()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);
        }

        [Test]
        public void TestGPTrackReorderNamesReload()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string filePath = File.GetOutputPath("SMK_TestGPTrackReorderNamesReload.sfc");

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            game.SaveRom(filePath);
            game = new Game(filePath); // Reload ROM

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);
        }

        [Test]
        public void TestGPTrackReorderRomData()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string filePathBefore = File.GetOutputPath("SMK_TestGPTrackReorderRomData_before.sfc");
            string filePatheAfter = File.GetOutputPath("SMK_TestGPTrackReorderRomData_after.sfc");

            game.SaveRom(filePathBefore);
            byte[] romBefore = System.IO.File.ReadAllBytes(filePathBefore);

            // Check the order value of each GP track
            Assert.AreEqual(0x07, romBefore[0x1EC1B]);
            Assert.AreEqual(0x13, romBefore[0x1EC1C]);
            Assert.AreEqual(0x10, romBefore[0x1EC1D]);
            Assert.AreEqual(0x11, romBefore[0x1EC1E]);
            Assert.AreEqual(0x0F, romBefore[0x1EC1F]);
            Assert.AreEqual(0x12, romBefore[0x1EC20]);
            Assert.AreEqual(0x01, romBefore[0x1EC21]);
            Assert.AreEqual(0x02, romBefore[0x1EC22]);
            Assert.AreEqual(0x03, romBefore[0x1EC23]);
            Assert.AreEqual(0x00, romBefore[0x1EC24]);
            Assert.AreEqual(0x0D, romBefore[0x1EC25]);
            Assert.AreEqual(0x0A, romBefore[0x1EC26]);
            Assert.AreEqual(0x0C, romBefore[0x1EC27]);
            Assert.AreEqual(0x09, romBefore[0x1EC28]);
            Assert.AreEqual(0x0E, romBefore[0x1EC29]);
            Assert.AreEqual(0x0B, romBefore[0x1EC2A]);
            Assert.AreEqual(0x06, romBefore[0x1EC2B]);
            Assert.AreEqual(0x08, romBefore[0x1EC2C]);
            Assert.AreEqual(0x04, romBefore[0x1EC2D]);
            Assert.AreEqual(0x05, romBefore[0x1EC2E]);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            game.SaveRom(filePatheAfter);
            byte[] romAfter = System.IO.File.ReadAllBytes(filePatheAfter);

            // Check the order value of each GP track
            Assert.AreEqual(0x07, romAfter[0x1EC1B]);
            Assert.AreEqual(0x10, romAfter[0x1EC1C]);
            Assert.AreEqual(0x11, romAfter[0x1EC1D]);
            Assert.AreEqual(0x0F, romAfter[0x1EC1E]);
            Assert.AreEqual(0x12, romAfter[0x1EC1F]);
            Assert.AreEqual(0x01, romAfter[0x1EC20]);
            Assert.AreEqual(0x02, romAfter[0x1EC21]);
            Assert.AreEqual(0x03, romAfter[0x1EC22]);
            Assert.AreEqual(0x13, romAfter[0x1EC23]);
            Assert.AreEqual(0x00, romAfter[0x1EC24]);
            Assert.AreEqual(0x0D, romAfter[0x1EC25]);
            Assert.AreEqual(0x0A, romAfter[0x1EC26]);
            Assert.AreEqual(0x0C, romAfter[0x1EC27]);
            Assert.AreEqual(0x09, romAfter[0x1EC28]);
            Assert.AreEqual(0x0E, romAfter[0x1EC29]);
            Assert.AreEqual(0x0B, romAfter[0x1EC2A]);
            Assert.AreEqual(0x06, romAfter[0x1EC2B]);
            Assert.AreEqual(0x08, romAfter[0x1EC2C]);
            Assert.AreEqual(0x04, romAfter[0x1EC2D]);
            Assert.AreEqual(0x05, romAfter[0x1EC2E]);
        }

        [Test]
        public void TestBattleTrackReorderReferences()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Track[] tracks =
            {
                game.TrackGroups[4][0],
                game.TrackGroups[4][1],
                game.TrackGroups[4][2],
                game.TrackGroups[4][3]
            };

            // Take Battle Course 4, and move it where Battle Course 1 is
            game.ReorderTracks(4, 3, 4, 0);

            Assert.AreEqual(tracks[3], game.TrackGroups[4][0]);
            Assert.AreEqual(tracks[0], game.TrackGroups[4][1]);
            Assert.AreEqual(tracks[1], game.TrackGroups[4][2]);
            Assert.AreEqual(tracks[2], game.TrackGroups[4][3]);
        }

        [Test]
        public void TestBattleTrackReorderNames()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Assert.AreEqual("Battle Course 1", game.TrackGroups[4][0].Name);
            Assert.AreEqual("Battle Course 2", game.TrackGroups[4][1].Name);
            Assert.AreEqual("Battle Course 3", game.TrackGroups[4][2].Name);
            Assert.AreEqual("Battle Course 4", game.TrackGroups[4][3].Name);

            // Take Battle Course 4, and move it where Battle Course 1 is
            game.ReorderTracks(4, 3, 4, 0);

            Assert.AreEqual("Battle Course 4", game.TrackGroups[4][0].Name);
            Assert.AreEqual("Battle Course 1", game.TrackGroups[4][1].Name);
            Assert.AreEqual("Battle Course 2", game.TrackGroups[4][2].Name);
            Assert.AreEqual("Battle Course 3", game.TrackGroups[4][3].Name);
        }

        [Test]
        public void TestBattleTrackReorderNamesReload()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string filePath = File.GetOutputPath("SMK_TestBattleTrackReorderNamesReload.sfc");

            Assert.AreEqual("Battle Course 1", game.TrackGroups[4][0].Name);
            Assert.AreEqual("Battle Course 2", game.TrackGroups[4][1].Name);
            Assert.AreEqual("Battle Course 3", game.TrackGroups[4][2].Name);
            Assert.AreEqual("Battle Course 4", game.TrackGroups[4][3].Name);

            // Take Battle Course 4, and move it where Battle Course 1 is
            game.ReorderTracks(4, 3, 4, 0);

            game.SaveRom(filePath);
            game = new Game(filePath); // Reload ROM

            Assert.AreEqual("Battle Course 4", game.TrackGroups[4][0].Name);
            Assert.AreEqual("Battle Course 1", game.TrackGroups[4][1].Name);
            Assert.AreEqual("Battle Course 2", game.TrackGroups[4][2].Name);
            Assert.AreEqual("Battle Course 3", game.TrackGroups[4][3].Name);
        }

        [Test]
        public void TestBattleTrackReorderRomData()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string filePathBefore = File.GetOutputPath("SMK_TestBattleTrackReorderRomData_before.sfc");
            string filePathAfter = File.GetOutputPath("SMK_TestBattleTrackReorderRomData_after.sfc");

            game.SaveRom(filePathBefore);
            byte[] romBefore = System.IO.File.ReadAllBytes(filePathBefore);

            // Check the order value of each battle track
            Assert.AreEqual(0x16, romBefore[0x1C15C]);
            Assert.AreEqual(0x17, romBefore[0x1C15D]);
            Assert.AreEqual(0x14, romBefore[0x1C15E]);
            Assert.AreEqual(0x15, romBefore[0x1C15F]);

            // Take Battle Course 4, and move it where Battle Course 1 is
            game.ReorderTracks(4, 3, 4, 0);

            game.SaveRom(filePathAfter);
            byte[] romAfter = System.IO.File.ReadAllBytes(filePathAfter);

            // Check the order value of each battle track
            Assert.AreEqual(0x15, romAfter[0x1C15C]);
            Assert.AreEqual(0x16, romAfter[0x1C15D]);
            Assert.AreEqual(0x17, romAfter[0x1C15E]);
            Assert.AreEqual(0x14, romAfter[0x1C15F]);
        }

        private void TestCodec(Region region, int offset, bool twice)
        {
            Game game = File.GetGame(region);

            byte[] romBuffer = File.ReadRom(region);
            byte[] originalCompressedData = Codec.GetCompressedChunk(romBuffer, offset);

            byte[] data = game.Decompress(offset, twice);
            byte[] newCompressedData = Codec.Compress(data, twice, true);

            // The new compressed data has to be different from the original compressed data,
            // since both didn't use the same compressor.
            Assert.AreNotEqual(originalCompressedData, newCompressedData);

            byte[] originalDecompressedData = Codec.Decompress(originalCompressedData);
            byte[] newDecompressedData = Codec.Decompress(newCompressedData);

            if (twice)
            {
                originalDecompressedData = Codec.Decompress(originalDecompressedData);
                newDecompressedData = Codec.Decompress(newDecompressedData);
            }

            // The decompressed data has to be the same, otherwise this implies that
            // simply recompressing unmodified data somehow modified it.
            Assert.AreEqual(originalDecompressedData, newDecompressedData);

            game.InsertData(newCompressedData, offset);

            string filePath = File.GetOutputPath($"SMK_{region}_{offset:X}_{twice}.sfc");
            game.SaveRom(filePath);

            byte[] newRomBuffer = System.IO.File.ReadAllBytes(filePath);
            byte[] newResavedCompressedData = Codec.GetCompressedChunk(newRomBuffer, offset);

            // Ensure the resaved ROM contains the new compressed data
            Assert.AreEqual(newCompressedData, newResavedCompressedData);
        }

        [Test]
        public void TestCodecPillarGraphicsUS()
        {
            TestCodec(Region.US, 0, false);
        }

        [Test]
        public void TestCodecPillarGraphicsEuro()
        {
            TestCodec(Region.Euro, 0, false);
        }

        [Test]
        public void TestCodecPillarGraphicsJap()
        {
            TestCodec(Region.Jap, 0, false);
        }

        [Test]
        public void TestCodecPodiumGraphicsUS()
        {
            TestCodec(Region.US, 0x737DA, true);
        }

        [Test]
        public void TestCodecPodiumGraphicsEuro()
        {
            TestCodec(Region.Euro, 0x737DA, true);
        }

        [Test]
        public void TestCodecPodiumGraphicsJap()
        {
            TestCodec(Region.Jap, 0x73A63, true);
        }
    }
}
