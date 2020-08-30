﻿using Decima;
using Decima.HZD;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace HZDCoreEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            //DecodeQuickTest();
            DecodeAllFilesTest();
            //DecodeLocalizationTest();
            //DecodeAudioTest();
        }

        static void DecodeQuickTest()
        {
            var files = new string[]
            {
                @"models\characters\humans\heads\baby\babyaloy\animation\parts\head_lx.core",
                @"models\characters\humans\hair\aloy\model\model.core",
                @"models\animation_managers\characters\animals\blackrat\blackrat_blackrat.core",
                @"sounds\music\world\world.core",
                @"shaders\ecotope\texturesetarrays\terrain_texture_array.core",
                @"animation_objects\ledge_climb\network\moaf_honst_fight_intro_network.core",
                @"movies\movielist.core",
                @"entities\trackedgamestats.core",
                @"localized\sentences\aigenerated\nora_adult_male1_1\sentences.core",
                @"interface\textures\markers\ui_marker_compass_bunker.core",
                @"levels\worlds\world\tiles\tile_x05_y-01\layers\lighting\cubezones\cubemapzone_07_cube.core",
                @"levels\worlds\world\tiles\tile_x05_y-01\layers\lighting\cubezones\cubezones_foundry_1.core",
                @"textures\lighting_setups\skybox\star_field.core",
                @"textures\base_maps\clouds_512.core",
                @"textures\detail_textures\buildingblock\buildingblock_detailmap_array.core",
                @"sounds\effects\dlc1\weapons\firebreather\wav\fire_loop_flames_01_m.core",
                @"models\building_blocks\nora\dressing\components\dressing_b125_c006_resource.core",
                @"entities\characters\models\humanoid_player.core",
                @"telemetry\designtelemetry.core",
                @"worlddata\worlddatapacking.core",
                @"entities\weapons\heavy_weapons\heavy_railgun_cables.core",
                @"entities\collectables\collectable_datapoint\datapoint_world.core",
                @"entities\shops\shops.core",
                @"levels\quests.core",
                @"levels\game.core",
                @"levels\worlds\world\leveldata\terrain.core",
                @"weather\defaultweathersystem.core",
                @"climates\regions\faro\faro_master_climate.core",
                @"entities\armor\outfits\outfits.core",
                @"models\weapons\raptordisc_playerversion\model\model.core",
                @"loadouts\loadout.core",
                @"levels\worlds\world\quests\mainquests\mq15_themountainthatfell_files\mq15_quest.core",
                @"entities\characters\models\humanoid_civilian.core",
                @"system\waves\white_noise_0dbfs.core",
            };

            foreach (string file in files)
            {
                string fullPath = Path.Combine(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\", file);
                Console.WriteLine(fullPath);

                try
                {
                    var objects = CoreBinary.Load(fullPath);
                }
                catch (InvalidDataException)
                {
                    // Broken pack file extraction
                }
            }
        }

        static void DecodeAllFilesTest()
        {
            var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\", "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                Console.WriteLine(file);

                try
                {
                    var objects = CoreBinary.Load(file);
                }
                catch (InvalidDataException)
                {
                    // Broken pack file extraction
                }
            }
        }

        static void DecodeLocalizationTest()
        {
            var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\localized\", "*", SearchOption.AllDirectories);
            var allStrings = new List<string>();

            foreach (string file in files)
            {
                Console.WriteLine(file);

                allStrings.Add("\n");
                allStrings.Add(file);

                try
                {
                    var objects = CoreBinary.Load(file);

                    foreach (var obj in objects)
                    {
                        if (obj is LocalizedTextResource asResource)
                            allStrings.Add(asResource.GetStringForLanguage(ELanguage.English));
                    }
                }
                catch (Exception)
                {
                    allStrings.Add("!!! File was skipped due to invalid data !!!");
                }
            }

            File.WriteAllLines(@"C:\text_data_dump.txt", allStrings);
        }

        static void DecodeAudioTest()
        {
            var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\sounds\music\menumusic\mainthememusic");
            //var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\sounds\music\loadingmusic\wav");
            //var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\sounds\effects\interface\hacking\wav");
            //var files = Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\sounds\effects\quest\mq13\wav");

            foreach (string file in files)
            {
                Console.WriteLine(file);

                var objects = CoreBinary.Load(file, false);
                var wave = objects[0] as WaveResource;

                if (wave == null)
                    continue;

                var data = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\Horizon Zero Dawn\Packed_DX12\extracted\sounds\music\loadingmusic\wav\temp.core");

                //using (var ms = new System.IO.MemoryStream(wave.WaveData.ToArray()))
                using (var ms = new MemoryStream(data))
                {
                    RawSourceWaveStream rs = null;

                    if (wave.Encoding == EWaveDataEncoding.MP3)
                        rs = new RawSourceWaveStream(ms, new Mp3WaveFormat(wave.SampleRate, wave.ChannelCount, wave.FrameSize, (int)wave.BitsPerSecond));
                    else if (wave.Encoding == EWaveDataEncoding.PCM)
                        rs = new RawSourceWaveStream(ms, new WaveFormat(wave.SampleRate, 16, wave.ChannelCount));

                    if (rs != null)
                    {
                        using (var wo = new WaveOutEvent())
                        {
                            wo.Init(rs);
                            wo.Play();

                            while (wo.PlaybackState == PlaybackState.Playing)
                                System.Threading.Thread.Sleep(50);
                        }
                    }
                }
            }
        }
    }
}
