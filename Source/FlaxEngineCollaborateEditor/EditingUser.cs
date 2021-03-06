using System;
using System.Collections.Generic;
using FlaxEditor;
using FlaxEditor.Gizmo;
using FlaxEditor.SceneGraph;
using FlaxEngine;
using MultiUsersEditing.Source.MultiUsersEditingPlugin;
using Object = FlaxEngine.Object;

namespace CollaboratePlugin
{
    public class EditingUser
    {
        public int Id;
        public String Name;
        public bool IsServer;
        public Color SelectionColor;
        [NoSerialize] private CustomOutliner _customOutliner;
        public CustomOutliner Outliner
        {
            get { return _customOutliner; }
            set
            {
                Editor.Instance.Windows.EditWin.Viewport.Task.CustomPostFx.Remove(_customOutliner);
                _customOutliner = value;
                Editor.Instance.Windows.EditWin.Viewport.Task.CustomPostFx.Add(value);
            }

        }
        public List<SceneGraphNode> Selection { get; } = new List<SceneGraphNode>();

        private Vector3 _position;
        public Vector3 Position
        {
            set
            {
                _position = value;
                Transform = Matrix.Transformation(Vector3.One, Orientation * Quaternion.RotationY(-90 * Mathf.DegreesToRadians), Position);
            }
            get { return _position; }
        }

        private Quaternion _orientation;

        public Quaternion Orientation
        {
            set
            {
                _orientation = value;
                Transform = Matrix.Transformation(Vector3.One, Orientation * Quaternion.RotationY(-90 * Mathf.DegreesToRadians), Position);
            }
            get { return _orientation; }
        }

        [NoSerialize]
        public Matrix Transform;

        public MaterialInstance Material { private set; get; }

        public EditingUser(int id, string username, Color selectionColor, bool server, Vector3 position, Quaternion orientation)
        {
            Id = id;
            Name = username;
            IsServer = server;
            SelectionColor = selectionColor;
            Position = position;
            Orientation = orientation;

            Scripting.InvokeOnUpdate(() =>
            {
                EditingSessionPlugin.Instance.CameraMaterial.WaitForLoaded();
                Material = EditingSessionPlugin.Instance.CameraMaterial.CreateVirtualInstance();
                Material.SetParameterValue("Color", SelectionColor);
            });
        }

        public void Close()
        {
            Editor.Instance.Windows.EditWin.Viewport.Task.CustomPostFx.Remove(_customOutliner);
            Object.Destroy(Material);
        }
    }
}