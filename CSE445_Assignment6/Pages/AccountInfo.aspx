<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountInfo.aspx.cs" Inherits="CSE445_Assignment6.AccountInfo" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Account Info - CSE445 Assignment 6</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>Account Info</h1>
            <div class="sub">
                Logged in as: <asp:Literal ID="litCurrentUser" runat="server" />
            </div>

            <div class="nav-links">
                <a class="btn" href="Default.aspx">Home</a>
                <a class="btn" href="Member.aspx">Member page</a>
                <a class="btn" href="Staff.aspx">Staff page</a>
                <a class="btn" href="Logout.aspx">Logout</a>
            </div>

            <div class="grid-cards">
                <!-- Card 1: Change username -->
                <div class="card">
                    <h3 style="margin-top: 0;">Change Username</h3>
                    <p class="small">
                        Update your username.
                    </p>

                    <div class="form-row">
                        <label>Current username</label>
                        <asp:Literal ID="litUserReadonly" runat="server" />
                    </div>

                    <div class="form-row">
                        <label for="txtNewUsername">New username</label>
                        <asp:TextBox ID="txtNewUsername" runat="server" />
                    </div>

                    <div class="form-row">
                        <asp:Button ID="btnChangeUsername" runat="server" CssClass="btn"
                            Text="Change Username" OnClick="btnChangeUsername_Click" />
                    </div>

                    <div class="form-row">
                        <asp:Literal ID="litUserMsg" runat="server" />
                    </div>
                </div>

                <!-- Card 2: Change password -->
                <div class="card">
                    <h3 style="margin-top: 0;">Change Password</h3>
                    <p class="small">
                        Change your password.
                    </p>

                    <div class="form-row">
                        <label for="txtCurrentPassword">Current password</label>
                        <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" />
                    </div>

                    <div class="form-row">
                        <label for="txtNewPassword">New password</label>
                        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" />
                    </div>

                    <div class="form-row">
                        <label for="txtConfirmPassword">Confirm new password</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                    </div>

                    <div class="form-row">
                        <asp:Button ID="btnChangePassword" runat="server" CssClass="btn"
                            Text="Change Password" OnClick="btnChangePassword_Click" />
                    </div>

                    <div class="form-row">
                        <asp:Literal ID="litPwdMsg" runat="server" />
                    </div>
                </div>

                <!-- Card 3: Change preferred Zip code -->
                <div class="card">
                    <h3 style="margin-top: 0;">Change Zipcode</h3>
                    <p class="small">
                        Update your Zipcode.
                    </p>

                    <div class="form-row">
                        <label>Current Zip code</label>
                        <asp:Literal ID="litZipReadOnly" runat="server" />
                    </div>

                    <div class="form-row">
                        <label for="txtNewZip">New Zip code</label>
                        <asp:TextBox ID="txtNewZip" runat="server" />
                    </div>

                    <div class="form-row">
                        <asp:Button ID="btnChangeZip" runat="server" CssClass="btn"
                            Text="Change Zip Code" OnClick="btnChangeZip_Click" />
                    </div>

                    <div class="form-row">
                        <asp:Literal ID="litZipMsg" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
