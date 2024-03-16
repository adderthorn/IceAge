using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mastonet;
using Mastonet.Entities;

namespace IceAge.Interop;
internal interface ITootFactory
{
    public HttpClient HttpClient { get; }
    public MastodonClient MastodonClient { get; }
    public AuthenticationClient AuthenticationClient { get; }
    public Auth Auth { get; }
}
