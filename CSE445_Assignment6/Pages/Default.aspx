<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSE445_Assignment6.Default" %>
<%@ Register Src="~/Controls/LoginPanel.ascx" TagPrefix="uc" TagName="LoginPanel" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link runat="server" href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>CSE445 Assignment 6 — Public page</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>CSE445 - Assignment 6 - News/Weather/Email Application</h1>

            <div class="grid">
                <!-- Top row: Login card and Component info card side by side -->
                <div class="grid-columns">

                    <!-- Left Card - Login -->
                    <div class="card">
                        <h2 style="margin-top: 0;">Login</h2>
                        <uc:LoginPanel ID="LoginPanel"
                            runat="server"
                            OnLogin="Login"
                            OnRegister="Register" />

                        <!-- Member/Staff buttons required on public page -->
                        <div style="margin-top: 12px;">
                            <a class="btn" href="Member.aspx">Member page</a>
                            <a class="btn" href="Staff.aspx" style="margin-left: 8px;">Staff page</a>
                        </div>
                    </div>

                    <!-- Right Card - Component explanation -->
                    <div class="card">
                        <h3 style="margin-top: 10px;">Local Component Layer components:</h3>
                        <br />
                        <dl class="kv">
                            <dt>DLL:</dt>
                            <dd>Implements hashing functions for login functionality.</dd>

                            <dt>User Control:</dt>
                            <dd>LoginPanel user control.</dd>

                            <dt>Cookies:</dt>
                            <dd>"Remember me" functionality for login as well as retaining ZIP input on TryIt page.</dd>
                        </dl>
                    </div>

                </div>

                <!-- Middle Card - Service Directory -->
                <div class="card" style="margin-top: 20px; padding-top: 20px;">
                    <h2 style="margin-top: 0;">Application and Components Summary Table</h2>

                    <div class="sub">
                        Public page - Deployment URL:
                        <asp:Literal ID="litDeployUrl" runat="server" />
                        <br />
                        Webstrar deployment URL: http://webstrar73.fulton.asu.edu/page0
                    </div>

                    <!-- hidden literal declarations -->
                    <div style="display: none">
                        <asp:Literal ID="litProvider1" runat="server" />
                        <asp:Literal ID="litLastZip" runat="server" />
                        <asp:Literal ID="litHashInput" runat="server" />
                        <asp:Literal ID="litLastUser" runat="server" />

                        <asp:Literal ID="litProvider2" runat="server" />
                        <asp:Literal ID="litProvider3" runat="server" />
                    </div>

                    <table class="dir">
                        <thead>
                            <tr>
                                <th style="width: 11%;">Provider</th>
                                <th style="width: 11%;">Type</th>
                                <th style="width: 44%;">Description / Operation / Params / Return</th>
                                <th style="width: 18%;">Try It</th>
                            </tr>
                        </thead>
                        <tbody>
                            <!-- Weather service -->
                            <tr>
                                <td><%: litProvider1.Text %></td>
                                <td>SVC (WSDL)</td>
                                <td class="small">
                                    Displays a 5-day weather forecast for a given ZIP code.<br />
                                    <b>Weather5day</b><br />
                                    Params: <code>zip - string</code><br />
                                    Return: <code>string[]</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#weather">TryIt</a>
                                    <span class="small muted">Endpoint:</span>
                                    <span class="small">
                                        <asp:Literal ID="litWxUrl" runat="server" /></span>
                                </td>
                            </tr>
                            <%-- Not using for Assignment 6
                            <!-- Wind service -->
                            <tr>
                                <td><%: litProvider1.Text %></td>
                                <td>SVC (WSDL)</td>
                                <td class="small">
                                    Displays average monthly wind intensity for a given ZIP code.<br />
                                    <b>WindIntensity</b><br />
                                    Params: <code>lat - decimal, lon - decimal</code><br />
                                    Return: <code>decimal</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#wind">TryIt</a>
                                    <span class="small muted">Endpoint:</span>
                                    <span class="small">
                                        <asp:Literal ID="litWindUrl" runat="server" /></span>
                                </td>
                            </tr>
                            

                            <!-- Solar service -->
                            <tr>
                                <td><%: litProvider1.Text %></td>
                                <td>SVC (WSDL)</td>
                                <td class="small">
                                    Displays annual-average solar energy data for a given ZIP code.<br />
                                    <b>SolarIntensity</b><br />
                                    Params: <code>lat - decimal, lon - decimal</code><br />
                                    Return: <code>decimal</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#solar">TryIt</a>
                                    <span class="small muted">Endpoint:</span>
                                    <span class="small">
                                        <asp:Literal ID="litSolarUrl" runat="server" /></span>
                                </td>
                            </tr>
                            --%>

                            <!-- DLL (hashing functions) -->
                            <tr>
                                <td><%: litProvider1.Text %></td>
                                <td>DLL</td>
                                <td class="small">
                                    DLL for hashing functions. Used in login for A6.<br />
                                    <b>Sha256Hex</b> and <b>Sha256Base64</b><br />
                                    Params: <code>plain: string</code><br />
                                    Return: <code>string</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#security">TryIt</a><br />
                                    <br />
                                </td>
                            </tr>

                            <!-- StockService(Diya) -->
                            <tr>
                                <td><%: litProvider2.Text %></td>
                                <td>SVC (WSDL)</td>
                                <td class="small">
                                    Real-time stock information and guidance service.<br />
                                    <b>DownloadStockInfo</b><br />
                                    Params: <code>symbol - string</code><br />
                                    Return: <code>string</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="Member.aspx#stock">TryIt</a>
                                    <span class="small muted">Endpoint:</span>
                                    <span class="small">
                                        <asp:Literal ID="litStockUrl" runat="server" /></span>
                                </td>
                            </tr>

                            <!-- Cookie component(Diya) -->
                            <tr>
                                <td><%: litProvider2.Text %></td>
                                <td>Cookie</td>
                                <td class="small">
                                    Stores the last searched stock symbol so the user can return and see
                                    their previous query prefilled.<br />
                                    Input: <code>symbol - string</code> from the stock textbox.<br />
                                    Return: (cookie stored on client side)
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#cookie">TryIt</a>
                                    <br />
                                    <br />
                                </td>
                            </tr>

                            <!-- Global.asax event handler(Diya) -->
                            <tr>
                                <td><%: litProvider2.Text %></td>
                                <td>Global.asax</td>
                                <td class="small">
                                    Global.asax event handler that tracks the total number of visits.<br />
                                    Input: none<br />
                                    Return: <code>int</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#global">TryIt</a>
                                    <br />
                                </td>
                            </tr>

                            <!-- NewsService (Elani) -->
                            <tr>
                                <td><%: litProvider3.Text %></td>
                                <td>SVC (WSDL)</td>
                                <td class="small">
                                    News service that returns a collection of news article URLs for one or more topics.<br />
                                    <b>NewsFocus</b><br />
                                    Params: <code>topics - string[]</code><br />
                                    Return: <code>string[]</code>
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#news">TryIt</a>
                                    <span class="small muted">Endpoint:</span>
                                    <span class="small">
                                        <asp:Literal ID="litNewsUrl" runat="server" /></span>
                                </td>
                            </tr>

                            <!-- Additional components (Elani) -->
                            <tr>
                                <td><%: litProvider3.Text %></td>
                                <td>Enter component type</td>
                                <td class="small">
                                    Enter Description/Operation/Params/Return
                                </td>
                                <td class="row-actions">
                                    <a class="btn" href="TryIt.aspx#comp7">TryIt</a>
                                    <span class="small muted">Enter endpoint</span>
                                </td>
                            </tr>

                        </tbody>
                    </table>
                </div>

                <br />

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
                        <br /><br />
                        <span>• To test the <b>DLL(unique component)</b>, simply navigate to the TryIt page via the TryIt buttons, enter an input into the SecurityLib - hasing (local DLL) section's input box, and select the preferred encryption type via the buttons to invoke.</span>
                        <br /><br />
                        <span>• To test the <b>Cookies</b>, simply navigate to the TryIt page via the TryIt buttons, enter an input into any box, invoke that service, and then leave the page(Close or use the back button). When you return to the same page, your inputs will be prefilled.</span>
                        <br /><br />
                        <span>• For the <b>User Control (LoginPanel)</b>, there is no actual test as the full functionality will be fully implemented in Assignment 6. For now, the Login function will only return an error for the Captcha, blank Username/Password, or that the function is under construction. The user control is implemented via the LoginPanel.ascx files in the Project within the Controls folder (CSE445_Assignment5/Controls/) where it may be inspected.</span>
                    </p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
