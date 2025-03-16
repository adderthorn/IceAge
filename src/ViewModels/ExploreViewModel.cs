using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceAge.TimelineFetcher;

namespace IceAge.ViewModels;
public class ExploreViewModel
{
    public ExploreFetcher Fetcher { get; }

    public ExploreViewModel(ExploreFetcher Fetcher)
    {
        this.Fetcher = Fetcher;
    }
}
