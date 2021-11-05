---
id: Pzdks
name: How the MusicTimer Works
file_version: 1.0.2
app_version: 0.6.4-0
file_blobs:
  Assets/Scripts/MusicTimer.cs: d04bc2bf65008db28901f90009999d31e7a3c425
---

The MusicTimer class is a timer-like class that ticks in according with the Tetris theme. We use it to synchronize the game according to the music.
<!-- NOTE-swimm-snippet: the lines below link your snippet to Swimm -->
### 📄 Assets/Scripts/MusicTimer.cs
```c#
⬜ 2      using UnityEngine;
⬜ 3      using System.Collections;
⬜ 4      
🟩 5      public class MusicTimer : MonoBehaviour
⬜ 6      {
⬜ 7          public AudioSource? AudioSource;
⬜ 8      
```

<br/>

It works by storing a constant recording of the times of each beat in the song (generated using a MIDI of the song with the script `📄 theme/extract_tempo.py` )

<br/>

The times of beats
<!-- NOTE-swimm-snippet: the lines below link your snippet to Swimm -->
### 📄 Assets/Scripts/MusicTimer.cs
```c#
⬜ 20     
⬜ 21         public const float OriginalLength = 68.62367f;
⬜ 22     
🟩 23         public readonly float[] OriginalBeatTimes =
🟩 24         {
🟩 25             0f,
🟩 26             0.428571f,
🟩 27             0.6428564999999999f,
🟩 28             0.857142f,
🟩 29             1.0714275f,
🟩 30             1.17857025f,
🟩 31             1.2857129999999999f,
🟩 32             1.4999984999999998f,
🟩 33             1.7142839999999997f,
🟩 34             2.1428549999999995f,
🟩 35             2.3571404999999994f,
🟩 36             2.5714259999999993f,
🟩 37             2.999996999999999f,
🟩 38             3.214282499999999f,
🟩 39             3.428567999999999f,
🟩 40             4.071424499999999f,
🟩 41             4.285709999999999f,
🟩 42             4.714280999999999f,
🟩 43             5.142851999999999f,
🟩 44             5.5714229999999985f,
🟩 45             5.999993999999998f,
🟩 46             6.857135999999998f,
🟩 47             7.499992499999998f,
🟩 48             7.7142779999999975f,
🟩 49             8.142848999999998f,
🟩 50             8.357134499999999f,
🟩 51             8.57142f,
🟩 52             9.2142765f,
🟩 53             9.428562000000001f,
🟩 54             9.857133000000001f,
🟩 55             10.071418500000002f,
🟩 56             10.285704000000003f,
🟩 57             10.714275000000002f,
🟩 58             10.928560500000003f,
🟩 59             11.142846000000004f,
🟩 60             11.571417000000004f,
🟩 61             11.999988000000004f,
🟩 62             12.428559000000003f,
🟩 63             12.857130000000003f,
🟩 64             13.714272000000003f,
🟩 65             14.142843000000003f,
🟩 66             14.357128500000004f,
🟩 67             14.571414000000004f,
🟩 68             14.785699500000005f,
🟩 69             14.892842250000005f,
🟩 70             14.999985000000004f,
🟩 71             15.214270500000005f,
🟩 72             15.428556000000006f,
🟩 73             15.857127000000006f,
🟩 74             16.071412500000005f,
🟩 75             16.285698000000004f,
🟩 76             16.714269000000005f,
🟩 77             16.928554500000004f,
🟩 78             17.142840000000003f,
🟩 79             17.785696500000004f,
🟩 80             17.999982000000003f,
🟩 81             18.428553000000004f,
🟩 82             18.857124000000006f,
🟩 83             19.285695000000008f,
🟩 84             19.71426600000001f,
🟩 85             20.57140800000001f,
🟩 86             21.21426450000001f,
🟩 87             21.42855000000001f,
🟩 88             21.85712100000001f,
🟩 89             22.07140650000001f,
🟩 90             22.285692000000008f,
🟩 91             22.92854850000001f,
🟩 92             23.142834000000008f,
🟩 93             23.57140500000001f,
🟩 94             23.78569050000001f,
🟩 95             23.999976000000007f,
🟩 96             24.42854700000001f,
🟩 97             24.642832500000008f,
🟩 98             24.857118000000007f,
🟩 99             25.28568900000001f,
🟩 100            25.71426000000001f,
🟩 101            26.14283100000001f,
🟩 102            26.571402000000013f,
🟩 103            26.999973000000015f,
🟩 104            28.285686000000016f,
🟩 105            29.142828000000016f,
🟩 106            29.999970000000015f,
🟩 107            30.857112000000015f,
🟩 108            31.714254000000015f,
🟩 109            32.571396000000014f,
🟩 110            33.42853800000002f,
🟩 111            33.857109000000015f,
🟩 112            35.14282200000002f,
🟩 113            35.99996400000002f,
🟩 114            36.85710600000002f,
🟩 115            37.714248000000026f,
🟩 116            38.142819000000024f,
🟩 117            38.57139000000002f,
🟩 118            39.428532000000025f,
🟩 119            40.28567400000003f,
🟩 120            41.57138700000003f,
🟩 121            41.78567250000003f,
🟩 122            41.999958000000035f,
🟩 123            42.21424350000004f,
🟩 124            42.32138625000004f,
🟩 125            42.42852900000004f,
🟩 126            42.64281450000004f,
🟩 127            42.857100000000045f,
🟩 128            43.28567100000004f,
🟩 129            43.499956500000046f,
🟩 130            43.71424200000005f,
🟩 131            44.14281300000005f,
🟩 132            44.35709850000005f,
🟩 133            44.57138400000005f,
🟩 134            45.21424050000005f,
🟩 135            45.428526000000055f,
🟩 136            45.85709700000005f,
🟩 137            46.28566800000005f,
🟩 138            46.71423900000005f,
🟩 139            47.14281000000005f,
🟩 140            47.99995200000005f,
🟩 141            48.64280850000005f,
🟩 142            48.85709400000005f,
🟩 143            49.28566500000005f,
🟩 144            49.499950500000054f,
🟩 145            49.71423600000006f,
🟩 146            50.35709250000006f,
🟩 147            50.57137800000006f,
🟩 148            50.99994900000006f,
🟩 149            51.21423450000006f,
🟩 150            51.42852000000006f,
🟩 151            51.85709100000006f,
🟩 152            52.07137650000006f,
🟩 153            52.285662000000066f,
🟩 154            52.714233000000064f,
🟩 155            53.14280400000006f,
🟩 156            53.57137500000006f,
🟩 157            53.99994600000006f,
🟩 158            54.428517000000056f,
🟩 159            55.28565900000005f,
🟩 160            55.499944500000055f,
🟩 161            55.71423000000006f,
🟩 162            55.92851550000006f,
🟩 163            56.03565825000006f,
🟩 164            56.14280100000006f,
🟩 165            56.357086500000065f,
🟩 166            56.57137200000007f,
🟩 167            56.999943000000066f,
🟩 168            57.21422850000007f,
🟩 169            57.42851400000007f,
🟩 170            57.85708500000007f,
🟩 171            58.07137050000007f,
🟩 172            58.285656000000074f,
🟩 173            58.928512500000075f,
🟩 174            59.14279800000008f,
🟩 175            59.571369000000075f,
🟩 176            59.99994000000007f,
🟩 177            60.42851100000007f,
🟩 178            60.85708200000007f,
🟩 179            61.71422400000007f,
🟩 180            62.35708050000007f,
🟩 181            62.571366000000076f,
🟩 182            62.999937000000074f,
🟩 183            63.214222500000076f,
🟩 184            63.42850800000008f,
🟩 185            64.07136450000007f,
🟩 186            64.28565000000008f,
🟩 187            64.71422100000008f,
🟩 188            64.92850650000008f,
🟩 189            65.14279200000009f,
🟩 190            65.57136300000009f,
🟩 191            65.7856485000001f,
🟩 192            65.9999340000001f,
🟩 193            66.4285050000001f,
🟩 194            66.8570760000001f,
🟩 195            67.28564700000011f,
🟩 196            67.71421800000012f
🟩 197        };
⬜ 198    
⬜ 199        public void Pause()
⬜ 200        {
```

<br/>

The Beat class represents some beat, stores the index of it and when it starts and ends
<!-- NOTE-swimm-snippet: the lines below link your snippet to Swimm -->
### 📄 Assets/Scripts/MusicTimer.cs
```c#
⬜ 206            Paused = false;
⬜ 207        }
⬜ 208    
🟩 209        public struct Beat
🟩 210        {
🟩 211            public float Start;
🟩 212            public float End;
🟩 213            public int Index;
🟩 214        };
⬜ 215    
⬜ 216        public Beat GetBeatAtTime(float playbackTime)
⬜ 217        {
```

<br/>

The GetBeatAtTime function takes the time of playback and returns the Beat the time is contained in
<!-- NOTE-swimm-snippet: the lines below link your snippet to Swimm -->
### 📄 Assets/Scripts/MusicTimer.cs
```c#
⬜ 213            public int Index;
⬜ 214        };
⬜ 215    
🟩 216        public Beat GetBeatAtTime(float playbackTime)
🟩 217        {
🟩 218            float currentBeatTime = 0;
🟩 219            float nextBeatTime = 0;
🟩 220            var audioSpeed = Speed;
🟩 221            int i;
🟩 222            for (i = 0; i < OriginalBeatTimes.Length; ++i)
🟩 223            {
🟩 224                currentBeatTime = OriginalBeatTimes[i] / audioSpeed + Offset;
🟩 225                if (i == OriginalBeatTimes.Length - 1)
🟩 226                {
🟩 227                    nextBeatTime = OriginalLength / audioSpeed + Offset;
🟩 228                }
🟩 229                else
🟩 230                {
🟩 231                    nextBeatTime = OriginalBeatTimes[i + 1] / audioSpeed + Offset;
🟩 232                }
🟩 233                if (playbackTime >= currentBeatTime && playbackTime < nextBeatTime)
🟩 234                {
🟩 235                    break;
🟩 236                }
🟩 237            }
🟩 238            return new Beat() { Start = currentBeatTime, End = nextBeatTime, Index = i };
🟩 239        }
⬜ 240    
⬜ 241        public float TimeToNextBeat()
⬜ 242        {
```

<br/>

This allows the Update function to calculate how much ticks there were.

<br/>

We calculate the difference in index between the last Beat reported from the timer to the current one to get the no. of ticks that passed since the last call to Update.
<!-- NOTE-swimm-snippet: the lines below link your snippet to Swimm -->
### 📄 Assets/Scripts/MusicTimer.cs
```c#
⬜ 261                return;
⬜ 262            }
⬜ 263            Beat currentBeat = GetBeatAtTime(AudioSource!.time);
🟩 264            int currentBeatIndex = currentBeat.Index;
🟩 265            int lastBeatIndex = _lastBeat.Value.Index;
⬜ 266            if (currentBeatIndex < lastBeatIndex)
⬜ 267            {
⬜ 268                // Wrap around the end of the beat array.
```

<br/>

This file was generated by Swimm. [Click here to view it in the app](https://app.swimm.io/repos/Z2l0aHViJTNBJTNBdW5pdHktdGV0cmlzJTNBJTNBc2FhcnJheg==/docs/Pzdks).