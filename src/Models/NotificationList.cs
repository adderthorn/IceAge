using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mastonet.Entities;

namespace IceAge.Models;
public partial class NotificationList : ObservableObject
{
    [ObservableProperty]
    public partial string LastReadId { get; set; }

    [ObservableProperty]
    public partial MastodonList<Notification> List { get; set; }

    public void MarkRead()
    {
        LastReadId = List
            .OrderByDescending(n => n.CreatedAt)
            .FirstOrDefault()
            .Id;
    }
}
