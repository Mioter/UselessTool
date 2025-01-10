using System.Windows;
using UselessTool.Bases.FileOperation;
using static UselessTool.Amusing.TextProcess.EmotionEmoticonGenerator;

namespace UselessTool.Amusing.TextProcess;

/// <summary>
/// 生成情绪颜文字的工具类。
/// </summary>
internal static class EmotionEmoticonManager
{
    private static readonly JsonConfig<Dictionary<EmotionType, List<string>>> JsonConfig = new(
        "Amusing",
        "emoticons_config.json"
    );

    /// <summary>
    /// 颜文字表情集合。
    /// </summary>
    public static Dictionary<EmotionType, List<string>> Emoticons { get; } = LoadEmoticons();

    /// <summary>
    /// 加载颜文字配置文件。
    /// </summary>
    /// <returns>返回一个包含情绪类型和对应颜文字列表的字典。</returns>
    private static Dictionary<EmotionType, List<string>> LoadEmoticons()
    {
        Dictionary<EmotionType, List<string>> emoticons;
        try
        {
            emoticons = JsonConfig.LoadFromJson() ?? GetDefaultConfig();
        }
        catch
        {
            emoticons = GetDefaultConfig();
            JsonConfig.SaveToJson(emoticons);
        }

        return emoticons;
    }

    /// <summary>
    /// 获取默认的颜文字配置。
    /// </summary>
    /// <returns>返回默认的颜文字配置。</returns>
    private static Dictionary<EmotionType, List<string>> GetDefaultConfig() =>
        new()
        {
            { EmotionType.Happy, ["(*^▽^*)", "(＾◡＾)", "٩(^o^)۶", "(づ￣ ³￣)づ"] },
            { EmotionType.Angry, ["(╬ಠ益ಠ)", "(ง •̀_•́)ง", "(╯°□°)╯︵ ┻━┻", "ლ(ಠ益ಠლ)"] },
            { EmotionType.Bored, ["(-_-)", "(・へ・?)", "¯\\_(ツ)_/¯", "(¬‿¬)"] },
            { EmotionType.Sad, ["(╥_╥)", "(T_T)", "(´；ω；`)", "(；一_一)"] },
        };

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

        SaveEmoticon();
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

        if (emoticons.Contains(emoticon))
            return;

        emoticons.Add(emoticon);
        SaveEmoticon();
    }

    /// <summary>
    /// 保存颜文字配置。
    /// </summary>
    private static void SaveEmoticon()
    {
        try
        {
            JsonConfig.SaveToJson(Emoticons);
        }
        catch (Exception ex)
        {
            // 可以在此处理错误
            MessageBox.Show($"{ex.Message}");
        }
    }
}
