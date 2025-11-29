<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="CSE445_Assignment6.Register" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Register - CSE445 Assignment 6</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-center">
            <div class="login">
            <div class="card">
                <div class="card-header">
                    <h1 style="margin: 0;">Register (Member self-enrollment)</h1>
                    <div class="sub">
                        Create a member account to access the Member page. A captcha is required when signing up.
                    </div>
                </div>
                <div class="card-body">
                    <div class="field">
                        <label for="<%= txtUser.ClientID %>">Username</label>
                        <asp:TextBox ID="txtUser" runat="server" />
                    </div>
                    <br />
                    <div class="field">
                        <label for="<%= txtPass.ClientID %>">Password</label>
                        <asp:TextBox ID="txtPass" runat="server" TextMode="Password" />
                    </div>

                    <div class="field">
                        <label for="<%= txtConfirm.ClientID %>">Confirm Password</label>
                        <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" />
                    </div>
                    <br />
                    <!-- captcha -->
                    <div class="field">
                        <label>Image verifier (captcha)</label>
                        <div class="captcha-wrap">
                            <img id="imgCaptcha" src="<%= ResolveUrl("~/Captcha.ashx") %>" alt="CAPTCHA" />
                            <asp:TextBox ID="txtCaptcha" runat="server" Width="90" placeholder="Enter code" />
                            <a href="javascript:void(0)"
                               class="captcha-refresh"
                               onclick="document.getElementById('imgCaptcha').src='<%= ResolveUrl("~/Captcha.ashx") %>?r=' + new Date().getTime();">
                                ↻
                            </a>
                        </div>
                    </div>
                    <br />
                    <div class="field">
                        <asp:Button ID="btnRegister" runat="server" CssClass="btn" Text="Register"
                            OnClick="btnRegister_Click" />
                    </div>
                    <br />
                    <div class="msg">
                        <asp:Literal ID="litMsg" runat="server" />
                    </div>

                    <div class="nav-links">
                        <a class="btn" href="Default.aspx">Back</a>
                        <a class="btn" href="Login.aspx">Go to Login</a>
                    </div>
                </div>
            </div>
        </div>
            </div>
    </form>
</body>
</html>
