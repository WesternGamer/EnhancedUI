var blockStates = null;

// Invoked from C#
// noinspection JSUnusedGlobalSymbols
async function stateUpdated() {
    const blockViews = $('#blocks');
    blockViews.empty();

    blockStates = await state.GetBlockStates();
    for (const entityId in blockStates) {
        renderBlock(blockViews, blockStates[entityId]);
    }
}

function renderBlock(parent, blockState) {
    let blockView = $('<div />');
    blockView.addClass('block');
    blockView.attr('id', 'block-' + blockState.EntityId);
    renderBlockInner(blockView, blockState);
    parent.append(blockView);
}

function renderBlockInner(blockView, blockState) {
    let id = $('<div />');
    id.addClass('entityId');
    id.text(blockState.EntityId);
    blockView.append(id);

    let type = $('<div />');
    type.addClass('type');
    type.text(blockState.ClassName + ' | ' + blockState.TypeId + ' | ' + blockState.SubtypeName);
    blockView.append(type);

    let name = $('<div />');
    name.addClass('name');
    name.text(blockState.Name);
    blockView.append(name);

    let properties = $('<div />')
    properties.addClass('properties');
    for (const propertyId in blockState.PropertyStates) {
        renderBlockProperty(properties, blockState.PropertyStates[propertyId])
    }
    blockView.append(properties)

    blockView.append($('<hr />'))
}

function renderBlockProperty(parent, propertyState) {
    let propertyView= $('<div />');
    propertyView.addClass('property');

    let cb, label;
    let value = $('<div />');
    value.addClass('value');
    switch(propertyState.TypeName) {
        case "Boolean":
            cb = $('<input />')
            cb.attr('id', propertyState.Id);
            cb.attr('type', 'checkbox');
            if (propertyState.Value) {
                cb.attr('checked', 'checked');
            }
            value.append(cb);

            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);

            break;

        case "Int64":
            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);

            cb = $('<input />')
            cb.attr('id', propertyState.Id);
            cb.attr('type', 'number');
            cb.attr('value', propertyState.Value == null ? '0' : propertyState.Value.toString());
            cb.attr('maxlength', '20');
            cb.attr('size', '20');
            value.append(cb);

            break;

        case "Single":
            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);

            cb = $('<input />')
            cb.attr('id', propertyState.Id);
            cb.attr('type', 'number');
            cb.attr('value', propertyState.Value == null ? '0.0' : propertyState.Value.toString());
            cb.attr('maxlength', '20');
            cb.attr('size', '20');
            value.append(cb);

            break;

        case "StringBuilder":
            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);

            cb = $('<input />')
            cb.attr('id', propertyState.Id);
            cb.attr('value', propertyState.Value == null ? '' : propertyState.Value.toString());
            cb.attr('maxlength', '65535');
            cb.attr('size', '100');
            value.append(cb);

            break;

        case "Color":
            // See https://bitbucket.org/chromiumembedded/cef/issues/899

            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id);
            value.append(label);

            cb = $('<input />')
            cb.attr('id', propertyState.Id);
            cb.attr('type', 'color');
            cb.attr('value', propertyState.Value == null ? '#ffffff' : propertyState.Value.toString());
            value.append(cb);

            break;

        default:
            label = $('<label />');
            label.attr('for', propertyState.Id);
            label.text(propertyState.Id + '[' + propertyState.TypeName + '] ');
            value.append(label);

            cb = $('<input />')
            cb.attr('readonly', 'readonly');
            cb.attr('id', propertyState.Id);
            cb.attr('value', propertyState.Value == null ? '' : propertyState.Value.toString());
            value.append(cb);

            break;
    }

    propertyView.append(value);

    parent.append(propertyView);
}

// Invoked from C#
// noinspection JSUnusedGlobalSymbols
async function blockStateUpdated(entityId) {
    // let blockState = await state.GetBlockState(entityId);
    // let blockView = $('#block-' + entityId);
    // if (blockView.length > 0) {
    //     renderBlockInner(blockView, blockState);
    // }
}