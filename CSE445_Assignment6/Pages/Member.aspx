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
                Welcome,
                <asp:Literal ID="litUser" runat="server" />!
            </div>

            <div class="nav-links">
                <a class="btn" href="Default.aspx">Home</a>
                <a class="btn" href="Logout.aspx">Logout</a>
                <a class="btn" href="Staff.aspx">Staff page</a>
                <a class="btn" href="AccountInfo.aspx">Account Info</a>
            </div>
            <div class="grid">
                <div class="grid-rows">
                    <!-- Title card -->
                    <div class="card" style="margin-top: 16px;">
                        <h2 style="margin-top: 0;">Member functions</h2>
                        <p class="small">
                            This page is intended to host features such as viewing personalized weather data, stock info and news articles
                        </p>
                    </div>

                    <!-- Card 1 - Weather -->
                    <div class="card">
                        <h2>Weather</h2>
                        <p>
                        <div class="row">
                            <span class="lbl">ZIP code:</span>
                            <br />
                            <asp:TextBox runat="server" ID="txtZip" Width="120" />
                            <asp:Button runat="server" ID="btnWeather" Text="Invoke" OnClick="btnWeather_Click" />
                        </div>
                        <!-- Output -->
                        <div class="row"><span class="lbl">Output:</span></div>
                        <div class="result" style="font-family: inherit;">
                            <asp:Literal ID="litWeather" runat="server" Mode="PassThrough"></asp:Literal>
                        </div>
                        <br />
                        <small>Source: NOAA National Weather Service</small>
                    </div>

                    <!-- Card 2 - StockService -->
                    <div class="card">
                        <h2 style="margin-top: 0;">Stock Info &amp; Analysis</h2>

                        <p class="small">
                            Input a stock symbol (AAPL, MSFT, etc.) and we will output
                    some info about it:
                        </p>

                        <!-- Component: Stock advice -->
                        <p>
                            Stock symbol:
                    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                            &nbsp;
                            <br />
                            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Give advice" />
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="Label2" runat="server"></asp:Label>
                        </p>
                    </div>

                    <!-- Card 3 - News articles -->
                    <div class="card" id="news">
                        <h2>NewsFocus</h2>
                        <br />
                        <div class="row">
                            <span class="lbl">Give us a topic(s) (comma-separated) and we'll show you some articles to read:</span>
                            <br />
                            <br />
                            <asp:TextBox runat="server" ID="txtNewsTopics" Width="420" />
                            <asp:Button runat="server" ID="btnNews" Text="Invoke" OnClick="btnNews_Click" />
                        </div>
                        <br />
                        <!-- Output -->
                        <div class="row"><span class="lbl">Output:</span></div>
                        <br />
                        <div class="result" style="font-family: inherit%;">
                            <asp:Literal ID="litNews" runat="server" Mode="PassThrough"></asp:Literal>
                        </div>
                        <small>Source: NewsData.io API.</small>
                    </div>

                    <!-- Card 4 - Global.asax Visitor Counter -->
                    <div class="card">
                        <h3 style="margin-top: 0;">Global.asax Event handler</h3>
                        <p class="small">
                            Global.asax Event handler - tracks total amount of users who use this application.
                            <br />
                            Testing: If you press the button once, open a new incognito window with this page link,
                    and hit the button again on the incognito browser, you can see it increased.
                        </p>
                        <p>
                            <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Get number of users" />
                        </p>
                        <p>
                            Total Application Visits:
                            <asp:Label ID="Label3" runat="server"></asp:Label>
                        </p>
                    </div>

                    <!-- Bottom Card - Testing Tips and Webstrar Info -->
                    <div style="margin-top: 0;" class="card">
                        <h1>***THIS APPLICATION IS DEPLOYED ON WEBSTRAR AS WELL***</h1>
                        <br />
                        <p style="padding-left: 2em;">
                            <span>• If you would like to test through <b>Webstrar</b>, visit: http://webstrar73.fulton.asu.edu/page0 while connected to Cisco AnyConnect using sslvpn.asu.edu/2fa as instructed in the Webstrar tutorial for this assignment.</span>
                        </p>
                        <h3>Testing Tips:</h3>
                        <p style="padding-left: 2em;">
                            <span>• To test the <b>WSDL services</b>, simply navigate to the TryIt page via the TryIt buttons, enter an input into the respective service's input box and Invoke.</span>
                            <br />
                            <br />
                            <span>• To test the <b>DLL</b>, simply navigate to the TryIt page via the TryIt buttons, enter an input into the SecurityLib - hasing (local DLL) section's input box, and select the preferred encryption type via the buttons to invoke.</span>
                            <br />
                            <br />
                            <span>• To test the <b>Cookies</b>, simply navigate to the TryIt page via the TryIt button, enter an input into any box, invoke that service, and then leave the page(Close or use the back button). When you return to the same page, your inputs will be prefilled.</span>
                            <br />
                            <br />
                            <span>• To test the <b>User Control (LoginPanel)</b>, login via the login section on the Default.aspx page or by pressing the "Log In" button which will direct you to the Login.aspx page. The user control is implemented via the LoginPanel.ascx files in the Project within the Controls folder (CSE445_Assignment6/Controls/) where it may be inspected.</span>
                            <br />
                            <br />
                            <span>• To test the <b>Global.asax</b>, navigate to the TryIt page via the TrIt button, or navigate to the Member page via the Member button. From there, you can interact with the User/Visitor counter that utilizes the Global.asax component.</span>
                        </p>
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
