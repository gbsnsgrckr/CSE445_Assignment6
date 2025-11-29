<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginPanel.ascx.cs" Inherits="CSE445_Assignment6.Controls.LoginPanel" %>



<link runat="server" href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
<div class="login">
    <!-- Username -->
    <label for="<%= txtUser.ClientID %>">Username</label>
    <asp:TextBox ID="txtUser" runat="server" />
    <br />
    <!-- Password -->
    <label for="<%= txtPass.ClientID %>">Password</label>
    <asp:TextBox ID="txtPass" runat="server" TextMode="Password" />
    <br />
    <!-- Captcha image verification -->
    <div id="captchaPanel" runat="server" class="captcha-wrap">
        <br />
        <!-- Captcha image -->
        <img id="imgCaptcha" src="<%= ResolveUrl("~/Captcha.ashx") %>" alt="CAPTCHA" />
        <!-- Captcha text input box -->
        <asp:TextBox ID="txtCaptcha" runat="server" Width="90" placeholder="Enter Captcha" />
        <a href="javascript:void(0)" class="captcha-refresh" onclick="document.getElementById('imgCaptcha').src='<%= ResolveUrl("~/Captcha.ashx") %>?r=' + new Date().getTime();">↻</a>
    </div>
    <br />
    <!-- Remember Me? checkbox -->
    <label class="remember-wrap">
        <asp:CheckBox ID="chkRememberMe" runat="server" />
        <span>Remember me? (cookie usage)</span>
        <br />
    </label>
    <br />
    <!-- Login and Register buttons -->
    <div class="row">
        <asp:Button ID="btnLogin" runat="server" CssClass="btn" Text="Log in" OnClick="btnLogin_Click" />
        <asp:Button ID="btnRegister" runat="server" CssClass="btn" Text="Register..." OnClick="btnRegister_Click" />
        <br />
    </div>

    <div class="row msg small">
        <asp:Literal ID="litMsg" runat="server" />
    </div>
</div>
