using System.IO;
using System.Text.Json;
using static UselessTool.Bases.SystemIO.FileSystemHelper;

namespace UselessTool.Amusing.TextProcess;

/// <summary>
/// 生成情绪颜文字的工具类。
/// </summary>
public static class EmotionEmoticonGenerator
{
    private static readonly Dictionary<EmotionType, List<string>> Emoticons;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    private static readonly string FilePath;

    static EmotionEmoticonGenerator()
    {
        FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Amusing", "emoticons_config.json");
        Emoticons = LoadEmoticons();
    }

    /// <summary>
    /// 表示情绪类型的枚举。
    /// </summary>
    public enum EmotionType
    {
        /// <summary>
        /// 开心。
        /// </summary>
        Happy,

        /// <summary>
        /// 生气。
        /// </summary>
        Angry,

        /// <summary>
        /// 无聊。
        /// </summary>
        Bored,

        /// <summary>
        /// 伤心。
        /// </summary>
        Sad,
    }

    /// <summary>
    /// 加载颜文字配置文件。
    /// </summary>
    /// <returns>返回一个包含情绪类型和对应颜文字列表的字典。</returns>
    private static Dictionary<EmotionType, List<string>> LoadEmoticons()
    {
        if (!File.Exists(FilePath))
        {
            var defaultConfig = GetDefaultConfig();
            SaveEmoticons(defaultConfig);
            return defaultConfig;
        }

        string jsonString = File.ReadAllText(FilePath);
        var config =
            JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString)
            ?? throw new InvalidOperationException("反序列化颜文字表情配置失败了，这是为什么，呜呜~");

        return config.ToDictionary(entry => Enum.Parse<EmotionType>(entry.Key), entry => entry.Value);
    }

    /// <summary>
    /// 获取默认的颜文字配置。
    /// </summary>
    /// <returns>返回默认的颜文字配置。</returns>
    private static Dictionary<EmotionType, List<string>> GetDefaultConfig() =>
        new()
        {
            {
                EmotionType.Happy,
                new List<string> { "(*^▽^*)", "(＾◡＾)", "٩(^o^)۶", "(づ￣ ³￣)づ" }
            },
            {
                EmotionType.Angry,
                new List<string> { "(╬ಠ益ಠ)", "(ง •̀_•́)ง", "(╯°□°)╯︵ ┻━┻", "ლ(ಠ益ಠლ)" }
            },
            {
                EmotionType.Bored,
                new List<string> { "(-_-)", "(・へ・?)", "¯\\_(ツ)_/¯", "(¬‿¬)" }
            },
            {
                EmotionType.Sad,
                new List<string> { "(╥_╥)", "(T_T)", "(´；ω；`)", "(；一_一)" }
            },
        };

    /// <summary>
    /// 在输入字符串后添加一个随机的颜文字。
    /// </summary>
    /// <param name="input">输入字符串。</param>
    /// <param name="emotionType">情绪类型。</param>
    /// <returns>返回添加了随机颜文字的字符串。</returns>
    public static string AddRandomEmoticon(string input, EmotionType emotionType)
    {
        if (!Emoticons.TryGetValue(emotionType, out var emoticons))
        {
            throw new ArgumentException("我还没有这种情绪...");
        }

        var random = new Random();
        int index = random.Next(emoticons.Count);
        return input + emoticons[index];
    }

    /// <summary>
    /// 移除指定情绪类型的颜文字并保存到配置文件。
    /// </summary>
    /// <param name="emotionType">情绪类型。</param>
    /// <param name="emoticon">要移除的颜文字。</param>
    /// <returns>如果颜文字存在并被移除，返回 true；否则返回 false。</returns>
    public static bool RemoveEmoticon(EmotionType emotionType, string emoticon)
    {
        if (!Emoticons.TryGetValue(emotionType, out var emoticons))
        {
            throw new ArgumentException("我还没有这种情绪...");
        }

        if (!emoticons.Remove(emoticon))
        {
            return false;
        }

        SaveEmoticons(Emoticons);
        return true;
    }

    /// <summary>
    /// 添加颜文字到指定情绪类型并保存到配置文件。
    /// </summary>
    /// <param name="emotionType">情绪类型。</param>
    /// <param name="emoticon">要添加的颜文字。</param>
    public static void AddEmoticon(EmotionType emotionType, string emoticon)
    {
        if (!Emoticons.TryGetValue(emotionType, out var emoticons))
        {
            throw new ArgumentException("我还没有这种情绪...");
        }

        if (!emoticons.Contains(emoticon))
        {
            emoticons.Add(emoticon);
            SaveEmoticons(Emoticons);
        }
    }

    /// <summary>
    /// 保存颜文字配置到文件。
    /// </summary>
    private static void SaveEmoticons(Dictionary<EmotionType, List<string>> emoticons)
    {
        var config = emoticons.ToDictionary(entry => entry.Key.ToString(), entry => entry.Value);
        string jsonString = JsonSerializer.Serialize(config, JsonSerializerOptions);

        EnsureDirectoryExists(FilePath);

        File.WriteAllText(FilePath, jsonString);
    }
}

