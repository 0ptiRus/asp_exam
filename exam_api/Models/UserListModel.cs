using System.Collections.Generic;

namespace exam_api.Models;

public class UserListModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PfpUrl { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public List<UserInListModel> Followers { get; set; } = new();
    public List<UserInListModel> Following { get; set; } = new();
}

public class UserInListModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PfpUrl { get; set; }
    public int FollowerCount { get; set; }
    public bool IsFollowing { get; set; }
}