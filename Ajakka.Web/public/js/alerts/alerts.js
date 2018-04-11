function loadRules(){
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }

    $.get({
        url: '/api/blacklist/page/'+currentPage,
        success: fillTableWithRules,
        error:showError
      });
    $.get({
        url:'/api/blacklist/pageCount',
        success:showPageCount
    })
} 

function loadActionTypes(){
    $.get({
        url:'/api/alerts/actionTypes',
        success: prefillActionTypes,
        error: function(err){
            $('#addNewRuleError').text('Alert actions cannot be loaded');
        }
    });
}

function prefillActionTypes(result){
    result.Content.forEach(function(actionType){
        actionTypes.push(actionType);
        $('#actionType').append($("<option></option>")
        .attr("value",actionType.TypeName)
        .text(actionType.Name)); 
    });
    onActionTypeValueChanged();
}

function onActionTypeValueChanged(){
    let selectedType = $('#actionType').val();
    $('#actionConfigurationContainer').empty();
    let typeDescriptor = actionTypes.find(function(item){
        return item.TypeName == selectedType; 
    });
    if(!typeDescriptor){
        return;
    }
    typeDescriptor.Properties.forEach(function(prop){createFormForActionProperty(prop);});
}

function createFormForActionProperty(prop){
    let propNameId = 'ap'+prop.Name;
    let fGroupDiv = $('#actionConfigurationContainer').append('<div class="form-group"></div>').children().last();
    let requiredClassName = '';
    let requiredText = '';
    if(prop.IsRequired){
        requiredClassName= ' required-property';
        requiredText = ' (required)';
    }

    fGroupDiv.append('<label for="'+propNameId +'">'+prop.DisplayName+requiredText+'</label>');
    
    let fInput = fGroupDiv.append('<input class="form-control action-property'+requiredClassName+'" type="text" id="'+propNameId +'"></input>').children().last();
    prop.assignedElement = fInput;
    fGroupDiv.append('<div class="small"><a class=" text-muted" href="'+prop.HintUrl+'" target="_blank">'+prop.Hint+'</a></div>');
    if(prop.IsRequired){
        fGroupDiv.append('<div class="invalid-feedback">The value cannot be empty.</div>')
    }
}

function showPageCount(pageCountResponse){
    $('#pageCount').text('');
    var pageCount = pageCountResponse.Content;
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }
    
    if(currentPage > 0){
        var previousPage = currentPage - 1;
        $('#pageCount').append('<a href="/settings/alerts?page='+previousPage+'"><i class="fas fa-caret-left"> ');    
    }
    currentPage++;
    $('#pageCount').append(' ' + currentPage + '/' + pageCount);

    if(currentPage < pageCount){
        var nextPage = currentPage;
        $('#pageCount').append('<a href="/settings/alerts?page='+nextPage+'"> <i class="fas fa-caret-right"> ');    
    }
}

function showError(error){
    console.log(error);
    $('#ruleListContainer').empty();
    $('#ruleListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.responseJSON.Content+'</td></tr>');
}

function fillTableWithRules(rules){
    $('#ruleListContainer').empty();

    allRules = [];
    var i = 1000;
    rules.Content.forEach(function(rule){
        i++;
        allRules.push(rule);
       
        $.get({
            url:'/api/alerts/linkedActions/'+rule.Id,
            async:false,
            success:function(result){
                let alertActionName = '';
                if(result.Content.length > 0){
                    alertActionName = result.Content[0].Name;
                }
                        
                var row = '<tr>';
                row += '<td>' + rule.Name + '</td>';
                row += '<td>' + rule.Pattern + '</td>';
                row += '<td>'+alertActionName+'</td>';
                row += '<td style="text-align:right"> <button class="btn btn-secondary" onclick="editRule(\''+rule.Id+'\')"><span data-toggle="tooltip" title="Edit"><i class="fas fa-edit"/><span></button> <button class="btn btn-secondary" href="#" onclick="deleteRule(\''+rule.Id+'\',\''+rule.Name+'\')"><span data-toggle="tooltip" title="Delete Rule"><i class="fas fa-trash-alt"/> </span></a></td>';
                row += '</tr>';
                $('#ruleListContainer').append(row);
            },
            error:function(err){console.log(err);}
        });
        
    });
    $('[data-toggle="tooltip"]').tooltip();
}

function deleteRule(id){
    $('.tooltip').hide();
    $.get({
        method:'DELETE',
        url: '/api/blacklist/rule/'+id,
        success: loadRules,
        error:showError
      });
}


function onAddNewDialogShown(){
    $('#editRuleName').trigger('focus');

    if(addNewDialogMode == 'edit')
        return;
    $('#editRuleName').val('');
    $('#editRulePattern').val('');
    $('.action-property').val('');

}

function clearValidationStyles(){
    $('#editRuleName').removeClass('is-invalid');
    $('#editRulePattern').removeClass('is-invalid');
    $('.required-property').removeClass('is-invalid');
  
    $('#addNewRuleError').hide();
}

function areNewRuleFieldsValid(){
    clearValidationStyles();
    
    var valid = true;
    var name = $('#editRuleName').val();
    if(name == '' || name == null){
        $('#editRuleName').addClass('is-invalid');
        valid = false;
    }
    var pattern = $('#editRulePattern').val();
    if(pattern == '' || pattern == null){
        $('#editRulePattern').addClass('is-invalid');
        valid = false;
    }

    
    $('.required-property').each(function(index){
        if($(this).val() == '' || $(this).val() == null){
            $(this).addClass('is-invalid');
            valid = false;
        }
    });

    return valid;
}

function addNewRule(){
    if(!areNewRuleFieldsValid())
        return false;
    if(addNewDialogMode == 'edit'){
        saveChangesToRule();
        return false;
    }
    let name = $('#editRuleName').val();
    let pattern = $('#editRulePattern').val();
    let addRulePost = new Promise(function(resolve, reject){
        $.post({
            url: '/api/blacklist/',
            data:{name:name, pattern:pattern},
            dataType:'json',
            success: resolve,
            error:reject
        });
    });

    let selectedActionType = $('#actionType').val();
    var actionToCreate = getActionToCreate(selectedActionType);
    addActionPost = new Promise(function(resolve, reject){
        $.post({
            url:'/api/alerts',
            data:actionToCreate,
            dataType:'json',
            success:resolve,
            error:reject
        });
    });
  
    addRulePost.then(function(rule){
        addActionPost.then(function(action){
            linkActionP(rule.Content.Id, action.Content.Id).then(function(result){
                $('#addNewRule').modal('hide');
                loadRules(); 
            })
            .catch(function(error){
                showRuleCreation(error);    
            });
        })
        .catch(function(error){
            showRuleCreation(error);

        });
    }).catch(function(error){
        showRuleCreation(error);
    });
}

function linkActionP(blacklistRuleId, linkActionId){
    return new Promise(function(resolve,reject){
        linkAction(blacklistRuleId, linkActionId, resolve, reject);
    });
}

function linkAction(blacklistRuleId, linkActionId, resolve, reject){
    $.ajax({
        method:'put',
        url: '/api/alerts/linkaction/'+blacklistRuleId +'/'+linkActionId,
        success:resolve,
        error:reject
    });
}

function getActionToCreate(typeName){
    let typeDescriptor = actionTypes.find(function(item){
        return item.TypeName == typeName; 
    });
    var actionConfiguration = '{';
    typeDescriptor.Properties.forEach(function(prop){
        let propNameId = '#ap'+prop.Name;
        let propNameVal = prop.assignedElement.val();
        actionConfiguration += prop.Name+':\''+propNameVal+'\',';
    });
    //remove last comma
    actionConfiguration = actionConfiguration.substring(0, actionConfiguration.length - 1);

    actionConfiguration += '}';

    return {
        name : typeDescriptor.Name,
        type : typeDescriptor.TypeName,
        configuration : actionConfiguration
    };
}

function showRuleCreation(error){
    console.log(error);
    $('#addNewRuleError').text(error.responseJSON.Content);
    $('#addNewRuleError').show();
}

setTimeout(loadRules, 100);
setTimeout(loadActionTypes, 100);

$('#addNewButton').on('click',function(){addNewDialogMode = 'add';});

$('#addNewRule').on('shown.bs.modal', function () {
    onAddNewDialogShown();
});
var actionTypes = new Array();
var allRules = new Array();
var addNewDialogMode ='add';