using System;
using System.Text;
using DoorRequest.API;
using NUnit.Framework;

namespace DoorRequest.Test
{
    public class OpenLDAPPasswordHelpersTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestUserAPasswordVerify()
        {
            var base64Decode =
                Convert.FromBase64String("e1NTSEF9NWJoRWJtK0s0S05xb0piZUZvSnVybHZSQmtBOXJuVlV4YlVYQUE9PQ==");
            var base64DecodedString = Encoding.UTF8.GetString(base64Decode);
            OpenLDAPPasswordHelpers.Compare("TestUserA", base64DecodedString);
        }


        [Test]
        public void TestUserBPasswordVerify()
        {
            var base64Decode = Convert.FromBase64String("e1NTSEF9WHBsVmFzdTJjRFc2NEtoeHc0ZU1ISFFOalRxN1lBYVRwVDh4VlE9PQ==");
            var base64DecodedString = Encoding.UTF8.GetString(base64Decode);
            OpenLDAPPasswordHelpers.Compare("TestUserB", base64DecodedString);
        }

        [Test]
        public void TestUserCPasswordVerify()
        {
            var base64Decode =
                Convert.FromBase64String("e1NTSEF9UndBZVdzNXp5V29WZlBXNEpkVnF4dFlCdDlDM3lMeTVHYjZjZlE9PQ==");
            var base64DecodedString = Encoding.UTF8.GetString(base64Decode);
            var exception = Assert.Throws<Exception>(() => OpenLDAPPasswordHelpers.Compare("TestUserC", base64DecodedString));
            Assert.That(exception.Message, Is.EqualTo("Invalid credentials"));
        }
    }
}