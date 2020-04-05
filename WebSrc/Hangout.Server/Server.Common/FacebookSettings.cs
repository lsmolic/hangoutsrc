using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
    public class FacebookSettings
    {
        private static string[] photosPermissions = new string[] { "offline_access", "photo_upload" };
        private static string[] photosPermissionsDescriptions = new string[] { "Allow us to access your Facebook photos", "Allow us to upload snapshots from your room" };
        private static string[] uploadSnapshotsPermissions = new string[] { "photo_upload" };
        private static string[] uploadSnapshotsPermissionsDescriptions = new string[] { "Allow us to upload snapshots from your room" };

        /// <summary>
        /// Gets a list of PermissionAndDescription objects representing all the permissions required to access Facebook photos in-room
        /// </summary>
        public static List<PermissionAndDescription> GetPhotosPermissions
        {
            get
            {
                List<PermissionAndDescription> photoPermissions = new List<PermissionAndDescription>();
                photoPermissions.Add(new PermissionAndDescription(AppPermission.offline_access, "Allow us to access your Facebook photos"));
                photoPermissions.Add(new PermissionAndDescription(AppPermission.photo_upload, "Allow us to upload snapshots from your room"));

                return photoPermissions;
            }
        }

        /// <summary>
        /// Gets a list of PermissionAndDescription objects representing all the permissions required to upload snapshots to Facebook
        /// </summary>
        public static List<PermissionAndDescription> GetUploadSnapshotsPermissions
        {
            get
            {
                List<PermissionAndDescription> uploadPermissions = new List<PermissionAndDescription>();
                uploadPermissions.Add(new PermissionAndDescription(AppPermission.photo_upload, "Allow us to upload snapshots from your room"));

                return uploadPermissions;
            }
        }

        public enum AppPermission
        {
            email,
            offline_access,
            status_update,
            photo_upload,
            create_listing,
            create_event,
            rsvp_event,
            sms
        }

        public class PermissionAndDescription
        {
            private AppPermission permission;
            private string description;

            /// <summary>
            /// Represents a Facebook permission type and an associated description
            /// </summary>
            /// <param name="permission"></param>
            /// <param name="description"></param>
            public PermissionAndDescription(AppPermission permission, string description)
            {
                this.permission = permission;
                this.description = description;
            }

            /// <summary>
            /// Gets the Facebook permission type
            /// </summary>
            public AppPermission Permission
            {
                get
                {
                    return permission;
                }
            }

            /// <summary>
            /// Gets the Facebook permission type description
            /// </summary>
            public string Description
            {
                get
                {
                    return description;
                }
            }
        }
    }
}
