var $sync = jQuery.noConflict();

$sync('[data-role="AddGlusterConfigNewNode"]').on("click", function () {
    var html = '<div class="input-control text" data-role="input-control"><input name="GlusterNode" type="text" value="@Current" style="width: 90%;"></div>';
    $sync('[data-role="GlusterConfigNewNodes"]').append(html);
});

$sync('[data-role="AddGlusterConfigNewVolume"]').on("click", function () {
    var html = '<div class="input-control text border-anthilla-gray" data-role="input-control"><input name="GlusterVolumeName" type="text" value="@Current.Name" style="width: 90%;"><input name="GlusterVolumeBrick" type="text" value="@Current.Brick" style="width: 90%;"><input name="GlusterVolumeMountPoint" type="text" value="@Current.MountPoint" style="width: 90%;"></div>';
    $sync('[data-role="GlusterConfigNewVolumes"]').append(html);
});