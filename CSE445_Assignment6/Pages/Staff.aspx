<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Staff.aspx.cs" Inherits="CSE445_Assignment6.Staff" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="~/Content/Dark.css" rel="stylesheet" type="text/css" />
    <title>Staff Page - CSE445 Assignment 6</title>
    <meta charset="utf-8" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <h1>Staff Page</h1>
            <div class="sub">
                Welcome,
                <asp:Literal ID="litUser" runat="server" />!
            </div>

            <div class="nav-links">
                <a class="btn" href="Default.aspx">Home</a>
                <a class="btn" href="Logout.aspx">Logout</a>
                <a class="btn" href="Member.aspx">Member page</a>
            </div>
            <div class="grid">
                <div class="grid-rows">
                    <!-- Title card  -->
                    <div class="card" style="margin-top: 16px;">
                        <h2 style="margin-top: 0;">Staff functions</h2>
                        <p class="small">
                            This Staff page is intended for administrative users. It can be used to review member
                    accounts, staff accounts, and also to reset user passwords when needed.
                        </p>
                        <div class="small">
                            <asp:Literal ID="litStatus" runat="server" />
                        </div>
                    </div>

                    <!-- Card 1 -->
                    <div class="grid-cards">
                        <div class="card">
                            <h3 style="margin-top: 0;">Card 1: Member Accounts (Member.xml)</h3>
                            <p class="small">
                                A users' usernames stored in Member.xml.
                            </p>
                            <div class="list-box">
                                <asp:Literal ID="litMembersList" runat="server" />
                            </div>
                        </div>

                        <!-- Card 2 -->
                        <div class="card">
                            <h3 style="margin-top: 0;">Card 2: Staff Accounts (Staff.xml)</h3>
                            <p class="small">
                                A list of users' usernames stored in Staff.xml.
                            </p>
                            <div class="list-box">
                                <asp:Literal ID="litStaffList" runat="server" />
                            </div>
                        </div>

                        <!-- Card 3 -->
                        <div class="card">
                            <h3 style="margin-top: 0;">Card 3: Reset User Password</h3>
                            <p class="small">
                                Staff users can reset the password for any user listed in <b>Users.xml</b>.
                            </p>

                            <div class="form-row">
                                <label for="ddlUsers">Select user</label>
                                <asp:DropDownList ID="ddlUsers" runat="server" />
                            </div>

                            <div class="form-row" style="margin-top: 8px;">
                                <label for="txtNewPassStaff">New password</label>
                                <asp:TextBox ID="txtNewPassStaff" runat="server" TextMode="Password" />
                            </div>

                            <div class="form-row" style="margin-top: 8px;">
                                <label for="txtConfirmPassStaff">Confirm new password</label>
                                <asp:TextBox ID="txtConfirmPassStaff" runat="server" TextMode="Password" />
                            </div>

                            <div class="form-row" style="margin-top: 10px;">
                                <asp:Button ID="btnResetPassword" runat="server" CssClass="btn"
                                    Text="Reset Password" OnClick="btnResetPassword_Click" />
                            </div>

                            <div class="form-row" style="margin-top: 8px;">
                                <asp:Literal ID="litResetMsg" runat="server" />
                            </div>
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
        </div>
    </form>


</body>
</html>
