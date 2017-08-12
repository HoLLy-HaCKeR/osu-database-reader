﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using osu_database_reader;
using osu_database_reader.Components.HitObjects;
using osu_database_reader.TextFiles;

namespace UnitTestProject
{
    [TestClass]
    public class TestsText
    {
        [TestInitialize]
        public void Init()
        {
            SharedCode.PreTestCheck();
        }

        [TestMethod]
        public void VerifyBigBlack()
        {
            //most people should have this map
            string beatmap = SharedCode.GetRelativeFile(@"Songs\41823 The Quick Brown Fox - The Big Black\The Quick Brown Fox - The Big Black (Blue Dragon) [WHO'S AFRAID OF THE BIG BLACK].osu");

            if (!File.Exists(beatmap))
                Assert.Inconclusive("Hardcoded path does not exist:  " + beatmap);
            if (!SharedCode.VerifyFileChecksum(beatmap, "2D687E5EE79F3862AD0C60651471CDCC"))
                Assert.Inconclusive("Beatmap was modified.");
            //TODO: check file to not be modified. if it is: inconclusive

            var bm = BeatmapFile.Read(beatmap);
            Debug.WriteLine("Beatmap version: " + bm.BeatmapVersion);

            Assert.IsTrue(bm.SectionGeneral.Count >= 2);   //disco prince only has 2
            Assert.IsTrue(bm.SectionMetadata.Count >= 4);  //disco prince only has 4
            Assert.IsTrue(bm.SectionDifficulty.Count >= 5);//disco prince only has 5

            //check General
            Assert.AreEqual(bm.SectionGeneral["AudioFilename"], "02 The Big Black.mp3");
            Assert.AreEqual(bm.SectionGeneral["AudioLeadIn"], "0");
            Assert.AreEqual(bm.SectionGeneral["PreviewTime"], "18957");
            
            //check MetaData
            Assert.AreEqual(bm.SectionMetadata["Title"], "The Big Black");
            Assert.AreEqual(bm.SectionMetadata["Artist"], "The Quick Brown Fox");
            Assert.AreEqual(bm.SectionMetadata["Creator"], "Blue Dragon");
            Assert.AreEqual(bm.SectionMetadata["Version"], "WHO'S AFRAID OF THE BIG BLACK");
            Assert.AreEqual(bm.SectionMetadata["Source"], string.Empty);
            Assert.AreEqual(bm.SectionMetadata["Tags"], "Onosakihito speedcore renard lapfox");

            //check Difficulty
            Assert.AreEqual(bm.SectionDifficulty["HPDrainRate"], "5");
            Assert.AreEqual(bm.SectionDifficulty["CircleSize"], "4");
            Assert.AreEqual(bm.SectionDifficulty["OverallDifficulty"], "7");
            Assert.AreEqual(bm.SectionDifficulty["SliderMultiplier"], "1.8");

            //check Events
            //TODO

            //check TimingPoints
            Assert.AreEqual(bm.TimingPoints.Count, 5);
            Assert.AreEqual(bm.TimingPoints[0].Time, 6966);
            Assert.AreEqual(bm.TimingPoints[1].Kiai, true);
            Assert.AreEqual(bm.TimingPoints[2].MsPerQuarter, -100); //means no timing change
            Assert.AreEqual(bm.TimingPoints[3].TimingChange, false);

            //check Colours
            //Combo1 : 249,91,9
            //(...)
            //Combo5 : 255,255,128
            Assert.AreEqual(bm.SectionColours["Combo1"], "249,91,91");
            Assert.AreEqual(bm.SectionColours["Combo5"], "255,255,128");
            Assert.AreEqual(bm.SectionColours.Count, 5);

            //check HitObjects
            Assert.AreEqual(bm.HitObjects.Count, 746);
            Assert.AreEqual(bm.HitObjects.Count(a => a is HitObjectCircle), 410);
            Assert.AreEqual(bm.HitObjects.Count(a => a is HitObjectSlider), 334);
            Assert.AreEqual(bm.HitObjects.Count(a => a is HitObjectSpinner), 2);

            //56,56,6966,1,4
            HitObjectCircle firstCircle = (HitObjectCircle) bm.HitObjects.First(a => a.Type.HasFlag(HitObjectType.Normal));
            Assert.AreEqual(firstCircle.X, 56);
            Assert.AreEqual(firstCircle.Y, 56);
            Assert.AreEqual(firstCircle.Time, 6966);
            Assert.AreEqual(firstCircle.HitSound, HitSound.Finish);

            //178,50,7299,2,0,B|210:0|300:0|332:50,1,180,2|0
            HitObjectSlider firstSlider = (HitObjectSlider)bm.HitObjects.First(a => a.Type.HasFlag(HitObjectType.Slider));
            Assert.AreEqual(firstSlider.X, 178);
            Assert.AreEqual(firstSlider.Y, 50);
            Assert.AreEqual(firstSlider.Time, 7299);
            Assert.AreEqual(firstSlider.HitSound, HitSound.None);
            Assert.AreEqual(firstSlider.CurveType, CurveType.Bezier);
            Assert.AreEqual(firstSlider.Points.Count, 3);
            Assert.AreEqual(firstSlider.RepeatCount, 1);
            Assert.AreEqual(firstSlider.Length, 180);

            //256,192,60254,12,4,61587
            HitObjectSpinner firstSpinner = (HitObjectSpinner)bm.HitObjects.First(a => a.Type.HasFlag(HitObjectType.Spinner));
            Assert.AreEqual(firstSpinner.X, 256);
            Assert.AreEqual(firstSpinner.Y, 192);
            Assert.AreEqual(firstSpinner.Time, 60254);
            Assert.AreEqual(firstSpinner.HitSound, HitSound.Finish);
            Assert.AreEqual(firstSpinner.EndTime, 61587);
            Assert.IsTrue(firstSpinner.IsNewCombo);
        }
    }
}