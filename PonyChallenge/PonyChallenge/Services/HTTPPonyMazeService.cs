using Newtonsoft.Json;
using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PonyChallenge.Services
{
    public class HTTPPonyMazeService : IPonyMazeService
    {
        readonly HttpClient client;
        readonly string baseURL = "https://ponychallenge.trustpilot.com";
        public HTTPPonyMazeService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 65536;
            client.BaseAddress = new Uri(baseURL);
        }

        private class TMazeDef
        {
            [JsonProperty("maze-width")]
            public int MazeWidth { get; set; }

            [JsonProperty("maze-height")]
            public int MazeHeight { get; set; }

            [JsonProperty("maze-player-name")]
            public string MazePlayerName { get; set; }

            [JsonProperty("difficulty")]
            public int Difficulty { get; set; }
        }

        private class TMazeIDResponse
        {
            [JsonProperty("maze-id")]
            public string MazeID { get; set; }
        }


        private class TMazeSnapshot
        {
            public int[] pony { get; set; }
            public int[] domokun { get; set; }
            public int[] endpoint { get; set; }
            public int[] size { get; set; }
            public int difficulty { get; set; }
            public string[][] data { get; set; }
            public string maze_id { get; set; }
            public TGameState gamestate { get; set; }
        }

        private class TMove
        {
            public string Direction { get; set; }
        }

        private class TMoveState
        {
            public string state { get; set; }
            public string stateresult { get; set; }
        }

        private class TGameState
        {
            public string state { get; set; }
            public string stateresult { get; set; }
        }


        async Task<Maze> IPonyMazeService.CreateMaze(int width, int height, string playerName, int difficulty)
        {
            TMazeDef mazeDef = new TMazeDef()
            {
                MazeWidth = width,
                MazeHeight = height,
                MazePlayerName = playerName,
                Difficulty = difficulty
            };

            StringContent postContent = new StringContent(JsonConvert.SerializeObject(mazeDef), Encoding.UTF8, "application/jason");
            HttpResponseMessage response = await client.PostAsync("/pony-challenge/maze", postContent);

            if (response.IsSuccessStatusCode)
            {
                TMazeIDResponse mazeIdResponse = JsonConvert.DeserializeObject<TMazeIDResponse>(await response.Content.ReadAsStringAsync());

                return new Maze()
                {
                    Id = mazeIdResponse.MazeID,
                    Width = width,
                    Height = height,
                    PlayerName = playerName,
                    Difficulty = difficulty
                };
            }
            throw new ApplicationException("Response unsuccessful: " + response.StatusCode.ToString());
        }

        async Task<MazeSnapshot> IPonyMazeService.GetSnapshot(string mazeId)
        {
            string snapshotString = await client.GetStringAsync("/pony-challenge/maze/" + mazeId);
            TMazeSnapshot snapshot = JsonConvert.DeserializeObject<TMazeSnapshot>(snapshotString);

            int width = snapshot.size[0];
            int height = snapshot.size[1];
            MazeSnapshot result = new MazeSnapshot()
            {
                Locations = new MazeLocation[width, height],
                DomokunPlacement = new MazePoint()
                {
                    X = (byte)(snapshot.domokun[0] % width),
                    Y = (byte)(snapshot.domokun[0] / width),
                },
                PonyPlacement = new MazePoint()
                {
                    X = (byte)(snapshot.pony[0] % width),
                    Y = (byte)(snapshot.pony[0] / width),
                },
                Exit = new MazePoint()
                {
                    X = (byte)(snapshot.endpoint[0] % width),
                    Y = (byte)(snapshot.endpoint[0] / width),
                }
            };

            for (int idx = 0; idx < width * height; idx++)
            {
                int x = idx % width;
                int y = idx % width;
                foreach (string wall in snapshot.data[idx])
                {
                    if (wall=="north")
                    {
                        result.Locations[x, y].NorthWall = true;
                        if (y > 0)
                            result.Locations[x, y - 1].SouthWall = true;
                    }

                    if (wall == "west")
                    {
                        result.Locations[x, y].WestWall = true;
                        if (x > 0)
                            result.Locations[x - 1, y].EastWall = true;
                    }

                    if (x == width - 1)
                        result.Locations[x, y].EastWall = true;

                    if (y == height - 1)
                        result.Locations[x, y].SouthWall = true;
                }
            }

            result.Locations[result.DomokunPlacement.X, result.DomokunPlacement.Y].ContainsDomokun = true;
            result.Locations[result.PonyPlacement.X, result.PonyPlacement.Y].ContainsPony = true;
            result.Locations[result.Exit.X, result.Exit.Y].IsExit = true;

            return result;
        }

        async Task IPonyMazeService.Move(string mazeId, string direction)
        {
            TMove move = new TMove()
            {
                Direction=direction
            };

            StringContent postContent = new StringContent(JsonConvert.SerializeObject(move), Encoding.UTF8, "application/jason");
            HttpResponseMessage response = await client.PostAsync("/pony-challenge/maze/" + mazeId, postContent);

            if (response.IsSuccessStatusCode)
            {
                TMoveState moveResponse = JsonConvert.DeserializeObject<TMoveState>(await response.Content.ReadAsStringAsync());

                return;
            }
            throw new ApplicationException("Response unsuccessful: " + response.StatusCode.ToString());
        }


    }
}
