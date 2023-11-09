// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.Internal;

namespace MrBoom
{
    public static class PlayFabPubSub
    {
        public static async Task<PlayFabResult<PubSubNegotiateResult>> Negotiate(PubSubNegotiateRequest request, object customData = null)
        {
            await default(PlayFabUtil.SynchronizationContextRemover);

            PlayFabAuthenticationContext playFabAuthenticationContext = request?.AuthenticationContext;

            object obj = await PlayFabHttp.DoPost("/PubSub/Negotiate", request, "X-EntityToken", playFabAuthenticationContext.EntityToken, null);

            if (obj is PlayFabError)
            {
                PlayFabError error = (PlayFabError)obj;
                PlayFabSettings.GlobalErrorHandler?.Invoke(error);
                return new PlayFabResult<PubSubNegotiateResult>
                {
                    Error = error,
                    CustomData = customData
                };
            }

            string serialized = (string)obj;
            PubSubNegotiateResult data = JsonSerializer.Deserialize<PubSubNegotiateResult>(serialized);
            return new PlayFabResult<PubSubNegotiateResult>
            {
                Result = data,
                CustomData = customData
            };
        }
    }

    public class PubSubNegotiateRequest : PlayFabRequestCommon
    {
        public Dictionary<string, string> CustomTags { get; set; }
    }

    public class PubSubNegotiateResult : PlayFabResultCommon
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
