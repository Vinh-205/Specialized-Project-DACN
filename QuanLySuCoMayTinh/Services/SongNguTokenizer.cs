using System.Globalization;
using System.Text;
using System.Text.Json;

namespace QuanLySuCoMayTinh.Services
{
    /// <summary>
    /// Tokenizer Unigram (SentencePiece, họ XLM-R) đọc từ tokenizer.json của mô hình đa ngôn ngữ.
    /// Chuẩn hóa: NFKC + gộp khoảng trắng (xấp xỉ bảng precompiled_charsmap; độ khớp được kiểm bằng id tham chiếu từ Python).
    /// </summary>
    public sealed class SongNguTokenizer
    {
        private readonly Dictionary<string, (int Id, double Score)> _vocab = new(StringComparer.Ordinal);
        private readonly int _maxPieceLen;
        private readonly double _unkScore;
        public const int BosId = 0, PadId = 1, EosId = 2, UnkId = 3;

        public SongNguTokenizer(string tokenizerJsonPath)
        {
            using var fs = File.OpenRead(tokenizerJsonPath);
            using var doc = JsonDocument.Parse(fs);
            var vocab = doc.RootElement.GetProperty("model").GetProperty("vocab");
            int id = 0; double min = 0; int maxLen = 1;
            foreach (var e in vocab.EnumerateArray())
            {
                var piece = e[0].GetString()!; var score = e[1].GetDouble();
                if (id > UnkId) { _vocab[piece] = (id, score); min = Math.Min(min, score); maxLen = Math.Max(maxLen, new StringInfo(piece).LengthInTextElements); }
                id++;
            }
            _maxPieceLen = Math.Min(maxLen, 32);
            _unkScore = min - 10.0;   // theo cách tính của SentencePiece/HF
        }

        private static string Normalize(string text)
        {
            var s = (text ?? "").Normalize(NormalizationForm.FormKC);
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                if (char.IsControl(c) && !char.IsWhiteSpace(c)) continue;
                sb.Append(char.IsWhiteSpace(c) ? ' ' : c);
            }
            return sb.ToString();
        }

        public int[] Encode(string text, int maxLength = 128)
        {
            var ids = new List<int> { BosId };
            foreach (var word in Normalize(text).Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                ids.AddRange(EncodeWord("▁" + word));
                if (ids.Count >= maxLength - 1) break;
            }
            if (ids.Count > maxLength - 1) ids.RemoveRange(maxLength - 1, ids.Count - (maxLength - 1));
            ids.Add(EosId);
            return ids.ToArray();
        }

        private List<int> EncodeWord(string w)
        {
            var runes = new List<string>();
            foreach (var r in w.EnumerateRunes()) runes.Add(r.ToString());
            int n = runes.Count;
            var best = new double[n + 1]; var back = new int[n + 1]; var bid = new int[n + 1];
            Array.Fill(best, double.NegativeInfinity); best[0] = 0;
            for (int i = 0; i < n; i++)
            {
                if (double.IsNegativeInfinity(best[i])) continue;
                var sb = new StringBuilder(); bool any = false;
                for (int j = i; j < n && j - i < _maxPieceLen; j++)
                {
                    sb.Append(runes[j]);
                    if (_vocab.TryGetValue(sb.ToString(), out var v))
                    {
                        any = any || j == i;
                        var sc = best[i] + v.Score;
                        if (sc > best[j + 1]) { best[j + 1] = sc; back[j + 1] = i; bid[j + 1] = v.Id; }
                    }
                }
                if (!any)   // ký tự lạ: dùng <unk> với điểm phạt
                {
                    var sc = best[i] + _unkScore;
                    if (sc > best[i + 1]) { best[i + 1] = sc; back[i + 1] = i; bid[i + 1] = UnkId; }
                }
            }
            var res = new List<int>();
            for (int p = n; p > 0; p = back[p]) res.Add(bid[p]);
            res.Reverse();
            // HF gộp các <unk> liền nhau thành một
            var merged = new List<int>();
            foreach (var x in res) if (!(x == UnkId && merged.Count > 0 && merged[^1] == UnkId)) merged.Add(x);
            return merged;
        }
    }
}
