<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CSE445_Assignment6.Login" %>
<%@ Register Src="~/Controls/LoginPanel.ascx" TagPrefix="uc" TagName="LoginPanel" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Login - CSE445 Assignment 6</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap page-center">
            <div class="card">
                <div class="card-header">
                    <h1 style="margin: 0;">Login</h1>
                    <div class="sub">
                        Please sign in to access the Member and Staff pages.
                    </div>
                </div>
                <div class="card-body">
                    <!-- Login user control -->
                    <uc:LoginPanel ID="LoginPanel1"
                                   runat="server"
                                   OnLogin="LoginPanel_Login"
                                   OnRegister="LoginPanel_Register" />
                    <br />
                    <div class="nav-links">
                        <a class="btn" href="Default.aspx">Back to Home</a>
                        <a class="btn" href="Member.aspx">Member page</a>
                        <a class="btn" href="Staff.aspx">Staff page</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
