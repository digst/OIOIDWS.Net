using System;
using System.Text;
using Digst.OioIdws.Wsp.BasicPrivilegeProfile;
using FluentAssertions;
using Xunit;

namespace Digst.OioIdws.Wsp.Test.BasicPrivilegeProfile
{
    public class BasicPrivilegeProfileDeserializerTest
    {
        [Fact]
        public void DeserializeBase64EncodedPrivilegeList_ReturnsExpected_WhenVersion101()
        {
            // Arrange
            var privilegeListBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"<?xml version='1.0' encoding='UTF-8'?>
                <bpp:PrivilegeList xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:bpp='http://itst.dk/oiosaml/basic_privilege_profile'>
	                <PrivilegeGroup Scope='urn:dk:gov:saml:CvrNumberIdentifier:34051178'>
		                <Privilege>http://bpp101.dk</Privilege>
	                </PrivilegeGroup>
                </bpp:PrivilegeList>"));
            
            var expectedScope = "urn:dk:gov:saml:CvrNumberIdentifier:34051178";
            var expectedPrivilege = "http://bpp101.dk";

            // Act
            var model = BasicPrivilegeProfileDeserializer.DeserializeBase64EncodedPrivilegeList(privilegeListBase64);

            // Assert
            model.Should().BeOfType<PrivilegeList101>();
            model.PrivilegeGroups.Should().ContainSingle();
            model.PrivilegeGroups.Should().ContainSingle(x => x.Scope == expectedScope);
            model.PrivilegeGroups[0].Privilege.Should().ContainSingle();
            model.PrivilegeGroups[0].Privilege.Should().ContainSingle(expectedPrivilege);
        }
        
        [Fact]
        public void DeserializeBase64EncodedPrivilegeList_ReturnsExpected_WhenVersion102()
        {
            // Arrange
            var privilegeListBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                @"<?xml version='1.0' encoding='UTF-8'?>
                <bpp:PrivilegeList xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:bpp='http://digst.dk/oiosaml/basic_privilege_profile'>
	                <PrivilegeGroup Scope='urn:dk:gov:saml:CvrNumberIdentifier:34051178'>
		                <Privilege>http://bpp12.dk</Privilege>
	                </PrivilegeGroup>
                </bpp:PrivilegeList>"));
            
            var expectedScope = "urn:dk:gov:saml:CvrNumberIdentifier:34051178";
            var expectedPrivilege = "http://bpp12.dk";

            // Act
            var model = BasicPrivilegeProfileDeserializer.DeserializeBase64EncodedPrivilegeList(privilegeListBase64);

            // Assert
            model.Should().BeOfType<PrivilegeList12>();
            model.PrivilegeGroups.Should().ContainSingle();
            model.PrivilegeGroups.Should().ContainSingle(x => x.Scope == expectedScope);
            model.PrivilegeGroups[0].Privilege.Should().ContainSingle();
            model.PrivilegeGroups[0].Privilege.Should().ContainSingle(expectedPrivilege);
        }
    }
}