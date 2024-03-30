using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mastonet;

namespace IceAge.Interop;
public class MastodonInterop
{
    public HttpClient HttpClient { get; }
    public MastodonClient MastodonClient { get; set; }

    public MastodonInterop()
    {
        HttpClient = new HttpClient();
    }
}
