function editRule(ruleId){
    var rule = allRules.find(function(item){
        return item.Id == ruleId;
    });
    $.get({
        url:'/api/alerts/'+rule.AlertActionIds[0],
        success:function(result){
            showEditDialog(rule, result.Content);
        },
        error:function(error){
            console.log(error);
            alert('Could not load data, check your network connection.');
        }
    });
}

function showEditDialog(rule, linkedAction){
    currentlyEditedRule = rule;
    addNewDialogMode = 'edit';
    $('#addNewRule').modal('show');
    $('#editRuleName').val(rule.Name);
    $('#editRulePattern').val(rule.Pattern);
    $('#actionType').val(linkedAction.TypeName);
    onActionTypeValueChanged();
    fillDynamicallyGeneratedFields(linkedAction);
    clearValidationStyles();
}

function fillDynamicallyGeneratedFields(linkedAction){
    let typeDescriptor = actionTypes.find(function(item){
        return item.TypeName == linkedAction.TypeName; 
    });
    typeDescriptor.Properties.forEach(function(prop){
        let elementId = '#ap'+prop.Name;
        $(elementId).val(linkedAction[prop.Name]);
    });
}

function saveChangesToRule(){
    let name = $('#editRuleName').val();
    let pattern = $('#editRulePattern').val();
  
    let selectedActionType = $('#actionType').val();
    var actionToUpdate = getActionToCreate(selectedActionType);
    actionToUpdate.actionId = currentlyEditedRule.AlertActionIds[0];

    var updateActionRequest = new Promise(function(resolve, reject){
        $.ajax({
            method:'put',
            url:'/api/alerts',
            data:actionToUpdate,
            dataType:'json',
            success:resolve,
            error:reject
        });
    });

    var updateRuleRequest = new Promise(function(resolve,reject){
        $.ajax({
            url: '/api/blacklist/',
            method: 'put',
            data:{name:name, pattern:pattern, actionId: currentlyEditedRule.AlertActionIds[0]},
            dataType:'json',
            success: resolve,
            error:reject
        });
    });
    
    updateActionRequest.then(function(actionResult){
        updateRuleRequest.then(function(ruleResult){
            $('#addNewRule').modal('hide');
            loadRules(); 
            currentlyEditedRule = null;
        })
        .catch(function(err){
            showRuleCreation(err);   
        });
    })
    .catch(function(err){
        showRuleCreation(err);   
    });
}

var currentlyEditedRule = null;