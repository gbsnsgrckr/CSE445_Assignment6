<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TryIt.aspx.cs" Inherits="CSE445_Assignment6.TryIt" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Assignment 6 – Integrated News/Stock/Weather Application</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>TryIt Page</h1>
            <div class="nav-links">
                <a class="btn" href="Default.aspx">Home</a>
            </div>
            <br />
            <div class="grid">

                <!-- Weather Service -->
                <div class="card">
                    <h2>Weather5day</h2>
                    <p>
                        Description: Returns a 5-day forecast for a U.S. ZIP code.<br />
                    </p>
                    <p>
                        Service URL: <span class="mono">
                            <asp:Label runat="server" ID="lblWeatherUrl" /></span><br />
                    </p>
                    <p>Signature: <code>string[] Weather5day(string zipcode)</code></p>
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

                <!-- SecurityLib - hashing functions-->
                <div class="card">
                    <h2>SecurityLib – hashing (local DLL)</h2>
                    <p>
                        Description: Provides hashing for login for A6. Generates a secure hash of the input text in hexadecimal or Base64.<br />
                    </p>
                    <div class="row">
                        <span class="lbl">Input:</span>
                        <asp:TextBox ID="txtHashInput" runat="server" Width="420" />
                    </div>
                    <br />
                    <div class="row">
                        <asp:Button ID="btnHashHex" runat="server" Text="Hash → Hex" OnClick="btnHashHex_Click" />
                        <asp:Button ID="btnHashB64" runat="server" Text="Hash → Base64" OnClick="btnHashB64_Click" />
                    </div>
                    <!-- Output -->
                    <div class="row"><span class="lbl">Output:</span></div>
                    <div class="result" style="font-family: inherit;">
                        <asp:Literal ID="litHash" runat="server" Mode="PassThrough"></asp:Literal>
                    </div>
                </div>

                <!-- StockService -->
                <div class="card">
                    <h2>StockService</h2>
                    <p>
                        Service URL: <span class="mono">
                            <asp:Label runat="server" ID="lblStockUrl" /></span><br />
                    </p>
                    <p>
                        Testing my service
                    </p>
                    <p>
                        Input a stock symbol (AAPL,MSFT,etc.) and we will output some advice about it:
                    </p>
                    <p>
                        Input symbol:
                    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                    </p>
                    <p>
                        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Give advice" />
                    </p>
                    <p>
                        <asp:Label ID="Label1" runat="server"></asp:Label>
                    </p>
                    <p>
                        <asp:Label ID="Label2" runat="server"></asp:Label>
                    </p>
                </div>

                <!-- Cookie Testing (separate component) -->
                <div class="card">
                    <h2>Cookie Testing</h2>
                    <p>
                        Component: Cookie Testing - This is what happens when user typically hits the 'Give Advice' Button
                    <br />
                        Enter symbol to be saved as a cookie:
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Set cookie" />
                        <br />
                        <br />
                        <asp:Label ID="Label4" runat="server"></asp:Label>
                        <br />
                        <br />
                    </p>
                </div>

                <!-- NewsService - NewsFocus -->
                <div class="card" id="news">
                    <h2>NewsFocus</h2>
                    <p>
                        Description: Returns a list of news article links for one or more topics.<br />
                    </p>
                    <p>
                        Service URL: <span class="mono">
                            <asp:Label runat="server" ID="lblNewsUrl" /></span><br />
                    </p>
                    <p>Signature: <code>string[] NewsFocus(string[] topics)</code></p>
                    <div class="row">
                        <span class="lbl">Topics (comma-separated):</span>
                        <asp:TextBox runat="server" ID="txtNewsTopics" Width="420" />
                        <asp:Button runat="server" ID="btnNews" Text="Invoke" OnClick="btnNews_Click" />
                    </div>
                    <!-- Output -->
                    <div class="row"><span class="lbl">Output:</span></div>
                    <div class="result" style="font-family: inherit;">
                        <asp:Literal ID="litNews" runat="server" Mode="PassThrough"></asp:Literal>
                    </div>
                    <small>Source: NewsData.io API.</small>
                </div>

                <!-- Global.asax Event handler -->
                <div class="card" id="global">
                    <h2>Global.asax Event handler</h2>
                    <p>
                        Description: Tracks total amount of users who use this application.<br />
                        Testing: If you press the button once, open a new incognito window with this page link,
                    and hit the button again on the incognito browser, you can see it increased.
                    </p>
                    <div class="row">
                        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Get number of users" />
                    </div>
                    <p>
                        Total Application Visits:
                    <asp:Label ID="Label3" runat="server"></asp:Label>
                    </p>
                </div>

            </div>

            <br />

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
                    <span>• To test the <b>DLL(unique component)</b>, simply navigate to the TryIt page via the TryIt buttons, enter an input into the SecurityLib - hasing (local DLL) section's input box, and select the preferred encryption type via the buttons to invoke.</span>
                    <br />
                    <br />
                    <span>• To test the <b>Cookies</b>, simply navigate to the TryIt page via the TryIt button, enter an input into any box, invoke that service, and then leave the page(Close or use the back button). When you return to the same page, your inputs will be prefilled.</span>
                    <br />
                    <br />
                    <span>• To test the <b>User Control (LoginPanel)</b>, login via the login section on the Default.aspx page or by pressing the "Log In" button which will direct you to the Login.aspx page. The user control is implemented via the LoginPanel.ascx files in the Project within the Controls folder (CSE445_Assignment5/Controls/) where it may be inspected.</span>
                    <br />
                    <br />
                    <span>• To test the <b>Global.asax</b>, navigate to the TryIt page via the TrIt button, or navigate to the Member page via the Member button. From there, you can interact with the User/Visitor counter that utilizes the Global.asax component.</span>
                </p>
            </div>
        </div>
    </form>
</body>
</html>
