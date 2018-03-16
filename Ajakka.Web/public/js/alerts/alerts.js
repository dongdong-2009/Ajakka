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
    typeDescriptor.Properties.forEach(function(prop){
        let propNameId = '#actionProp'+prop.Name;
        $('#actionConfigurationContainer').append('<div class="form-group"><label for="'+propNameId +'">'+prop.DisplayName+'</label><input class="form-control" type="text" id="'+propNameId +'"></input></div>')
    });
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
    $('#ruleListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.statusText+'</td></tr>');
}

function fillTableWithRules(rules){
    $('#ruleListContainer').empty();

    rules.Content.forEach(function(rule){
        var activeAlerting = 'yes';
        if(rule.AlertActionIds.length == 0){
            activeAlerting = 'no';
        }
        var row = '<tr>';
        row += '<td>' + rule.Name + '</td>';
        row += '<td>' + rule.Pattern + '</td>';
        row += '<td>' + activeAlerting + '</td>';
        row += '<td style="text-align:right"> <button class="btn btn-secondary" onclick="configureAlerts(\''+rule.Id+'\')"><span data-toggle="tooltip" title="Edit"><i class="fas fa-edit"/><span></button> <button class="btn btn-secondary" href="#" onclick="deleteRule(\''+rule.Id+'\',\''+rule.Name+'\')"><span data-toggle="tooltip" title="Delete Rule"><i class="fas fa-trash-alt"/> </span></a></td>';
        row += '</tr>';
        $('#ruleListContainer').append(row);
        
    });
    $('[data-toggle="tooltip"]').tooltip()
}

function deleteRule(id){
    
    $.get({
        method:'DELETE',
        url: '/api/blacklist/rule/'+id,
        success: loadRules,
        error:showError
      });
}

function addNewRule(){

    $('#editRuleName').removeClass('is-invalid');
    $('#editRulePattern').removeClass('is-invalid');
   
    $('#addNewRuleError').hide();

    var name = $('#editRuleName').val();
    if(name == '' || name == null){
        $('#editRuleName').addClass('is-invalid');
        return false;
    }
    var pattern = $('#editRulePattern').val();
    if(pattern == '' || pattern == null){
        $('#editRulePattern').addClass('is-invalid');
        return false;
    }
    
    var post = new Promise(function(resolve, reject){
        $.post({
            url: '/api/blacklist/',
            data:{name:name, pattern:pattern},
            dataType:'json',
            success: resolve,
            error:reject
        });
    });
    post.then(function(result){
        $('#addNewRule').modal('hide');
       
        loadRules(); 
    }).catch(function(error){
        showRuleCreation(error);
        
    });
}

function showRuleCreation(error){
    console.log(error);
    $('#addNewRuleError').text(error.responseText);
    $('#addNewRuleError').show();
}

function configureAlerts(ruleId){
    window.location.href='/settings/alerts/configure/' + ruleId;
}

setTimeout(loadRules, 100);
setTimeout(loadActionTypes, 100);

$('#addNewRule').on('shown.bs.modal', function () {
    $('#editRuleName').val('');
    $('#editRulePattern').val('');
    $('#editRuleName').trigger('focus');
   
})
var actionTypes = new Array();