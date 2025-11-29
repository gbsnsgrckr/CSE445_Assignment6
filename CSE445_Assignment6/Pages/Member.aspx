<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Member.aspx.cs" Inherits="CSE445_Assignment6.Member" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Member Page - CSE445 Assignment 6</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>Member Page</h1>
            <div class="sub">
                Welcome, <asp:Literal ID="litUser" runat="server" />!
            </div>

            <div class="nav-links">
                <a class="btn" href="Default.aspx">Home</a>
                <a class="btn" href="Logout.aspx">Logout</a>
                <a class="btn" href="Staff.aspx">Staff page</a>
                <a class="btn" href="AccountInfo.aspx">Account Info</a>
            </div>

            <!-- Title card -->
            <div class="card" style="margin-top: 16px;">
                <h2 style="margin-top: 0;">Member functions</h2>
                <p class="small">
                    This page is intended to host features such as viewing personalized weather data, stock info and new articles
                </p>
            </div>

            <!-- Card 1 - Weather -->
            <div class="card">
                <div class="row">
                    <span class="lbl">ZIP code:</span>
                    <asp:TextBox runat="server" ID="txtZip" Width="120" />
                    <asp:Button runat="server" ID="btnWeather" Text="Invoke" OnClick="btnWeather_Click" />
                </div>
                <!-- Output -->
                <div class="row"><span class="lbl">Output:</span></div>
                <div class="result" style="font-family: inherit;">
                    <asp:Literal ID="litWeather" runat="server" Mode="PassThrough"></asp:Literal>
                </div>
                <small>Source: NOAA National Weather Service</small>
            </div>

            <!-- Card 2 - Stock TryIt + cookies + global counter -->
            <div class="card">
                <h3 style="margin-top: 0;">Stock Info &amp; Usage</h3>

                <p class="small">
                    Input a stock symbol (AAPL, MSFT, etc.) and we will output
                    some info about it:
                </p>

                <!-- Component: Stock advice -->
                <p>
                    Stock symbol:
                    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                    &nbsp;
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Give advice" />
                </p>
                <p>
                    <asp:Label ID="Label1" runat="server"></asp:Label>
                </p>
                <p>
                    <asp:Label ID="Label2" runat="server"></asp:Label>
                </p>

                <!-- Component 2: Global Event handler -->
                <p class="small">
                    Component 2: Global Event handler - tracks total amount of users who use this application.
                    Testing: If you press the button once, open a new incognito window with this page link,
                    and hit the button again on the incognito browser, you can see it increased.
                </p>
                <p>
                    <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Get number of users" />
                </p>
                <p>
                    Total Application Visits: <asp:Label ID="Label3" runat="server"></asp:Label>
                </p>
            </div>

            <!-- Card 3 - News articles -->
            <div class="card" id="news">
                <h2>NewsFocus</h2>
                <br />
                <div class="row">
                    <span class="lbl">Give us a topic(s) (comma-separated) and we'll show you some articles to read:</span>
                    <br />
                    <asp:TextBox runat="server" ID="txtNewsTopics" Width="420" />
                    <asp:Button runat="server" ID="btnNews" Text="Invoke" OnClick="btnNews_Click" />
                </div>
                <!-- Output -->
                <div class="row"><span class="lbl">Output:</span></div>
                <br />
                <div class="result" style="font-family: inherit;">
                    <asp:Literal ID="litNews" runat="server" Mode="PassThrough"></asp:Literal>
                </div>
                <small>Source: NewsData.io API.</small>
            </div>

        </div>
    </form>
</body>
</html>
