using static UselessTool.Amusing.TextProcess.EmotionEmoticonGenerator;

namespace UselessTool.Amusing.TextProcess;

public static class EmotionEmoticonGenerator
{
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
    /// 附加处理。
    /// </summary>
    public enum AdditionalRules
    {
        /// <summary>
        /// 喵~
        /// </summary>
        Cat,

        /// <summary>
        /// 不要惹！
        /// </summary>
        Lady,

        /// <summary>
        /// 哼！才不是我的问题呢！
        /// </summary>
        Tsundere,
    }

    /// <summary>
    /// 在输入字符串后添加一个随机的颜文字，可附加其他规则。
    /// </summary>
    /// <param name="input">输入字符串。</param>
    /// <param name="emotionType">情绪类型，默认开心。</param>
    /// <param name="additionalRules">附加规矩列表。</param>
    /// <returns>返回添加了随机颜文字的字符串。</returns>
    public static string AddRandomEmoticon(
        string input,
        EmotionType emotionType = EmotionType.Happy,
        params AdditionalRules[] additionalRules
    )
    {
        if (additionalRules.Length != 0)
        {
            input = ApplyAdditionalRules(input, emotionType, additionalRules);
        }

        if (!EmotionEmoticonManager.Emoticons.TryGetValue(emotionType, out var emoticons))
        {
            throw new ArgumentException("我还没有这种情绪...");
        }

        var random = new Random();
        int index = random.Next(emoticons.Count);
        return input + emoticons[index];
    }

    /// <summary>
    /// 按照附加规则处理文本。
    /// </summary>
    /// <param name="input">输入字符串。</param>
    /// <param name="emotionType">情绪类型。</param>
    /// <param name="additionalRules">附加规则。</param>
    /// <returns>返回处理后的字符串。</returns>
    public static string ApplyAdditionalRules(
        string input,
        EmotionType emotionType,
        params AdditionalRules[] additionalRules
    )
    {
        return additionalRules.Aggregate(
            input,
            (current, rule) =>
                rule switch
                {
                    AdditionalRules.Cat => AdditionalRuleProcessor.ApplyCatRule(current, emotionType),
                    AdditionalRules.Lady => AdditionalRuleProcessor.ApplyLadyRule(current),
                    AdditionalRules.Tsundere => AdditionalRuleProcessor.ApplyTsundereRule(current, emotionType),
                    _ => current,
                }
        );
    }
}

internal static class AdditionalRuleProcessor
{
    public static string ApplyCatRule(string input, EmotionType emotionType)
    {
        char punctuation = emotionType switch
        {
            EmotionType.Angry => '！',
            EmotionType.Happy => '~',
            _ => '。',
        };

        if (char.IsPunctuation(input[^1]))
        {
            input = input[..^1];
        }

        return input + "喵" + punctuation;
    }

    public static string ApplyLadyRule(string input)
    {
        string[] sentences = input.Split('。');
        for (int i = 0; i < sentences.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(sentences[i]))
            {
                sentences[i] = sentences[i].TrimEnd() + "惹。";
            }
        }
        return string.Join('。', sentences);
    }

    public static string ApplyTsundereRule(string input, EmotionType emotionType)
    {
        switch (emotionType)
        {
            case EmotionType.Sad:
                input += "唔~才不是我的问题呢。";
                break;
            case EmotionType.Angry:
                input += "这一定是你的问题，哼！";
                break;
            case EmotionType.Happy:
                input += "欸嘿！";
                break;
            case EmotionType.Bored:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(emotionType), emotionType, null);
        }
        return input;
    }
}
