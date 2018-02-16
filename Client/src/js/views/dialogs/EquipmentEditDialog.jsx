import React from 'react';

import { connect } from 'react-redux';

import { Grid, Row, Col } from 'react-bootstrap';
import { Form, FormGroup, HelpBlock, ControlLabel } from 'react-bootstrap';

import * as Api from '../../api';

import EditDialog from '../../components/EditDialog.jsx';
import FormInputControl from '../../components/FormInputControl.jsx';

import { isBlank, notBlank } from '../../utils/string';
import { isValidYear } from '../../utils/date';

var EquipmentEditDialog = React.createClass({
  propTypes: {
    equipment: React.PropTypes.object,

    onSave: React.PropTypes.func.isRequired,
    onClose: React.PropTypes.func.isRequired,
    show: React.PropTypes.bool,
  },

  getInitialState() {
    return {
      isNew: this.props.equipment.id === 0,

      serialNumber: this.props.equipment.serialNumber || '',
      make: this.props.equipment.make || '',
      size: this.props.equipment.size || '',
      model: this.props.equipment.model || '',
      year: this.props.equipment.year || '',
      licencePlate: this.props.equipment.licencePlate || '',
      type: this.props.equipment.type || '',

      serialNumberError: null,
      yearError: null,
    };
  },

  componentDidMount() {
    this.input.focus();
  },

  updateState(state, callback) {
    this.setState(state, callback);
  },

  didChange() {
    if (this.state.serialNumber !== this.props.equipment.serialNumber) { return true; }
    if (this.state.make !== this.props.equipment.make) { return true; }
    if (this.state.size !== this.props.equipment.size) { return true; }
    if (this.state.model !== this.props.equipment.model) { return true; }
    if (this.state.year !== this.props.equipment.year) { return true; }
    if (this.state.licencePlate !== this.props.equipment.licencePlate) { return true; }
    if (this.state.type !== this.props.equipment.type) { return true; }    

    return false;
  },

  isValid() {
    this.setState({
      serialNumberError: null,
      licencePlateError: null,
    });

    var valid = true;

    if (isBlank(this.state.serialNumber)) {
      this.setState({ serialNumberError: 'Serial number is required' });
      valid = false;
    }

    if (notBlank(this.state.year) && !isValidYear(this.state.year)) {
      this.setState({ yearError: 'This is not a valid year.' });
      valid = false;
    }

    return valid;
  },

  onSave() {
    Api.equipmentDuplicateCheck(this.props.equipment.id, this.state.serialNumber).then((response) => {
      if (response.data.length > 0) {
        var districts = response.data.map((district) => {
          return district.districtName;
        });
        this.setState({ 
          serialNumberError: `Serial number is currently in use in the following district(s): ${districts.join(', ')}`,
        });
        return;
      }
      this.props.onSave({ ...this.props.equipment, ...{
        serialNumber: this.state.serialNumber,
        make: this.state.make,
        size: this.state.size,
        model: this.state.model,
        year: this.state.year,
        licencePlate: this.state.licencePlate,
        type: this.state.type,
      }});
    });
  },

  render() {
    return <EditDialog id="equipment-edit" show={ this.props.show }
      onClose={ this.props.onClose } onSave={ this.onSave } didChange={ this.didChange } isValid={ this.isValid }
      title= { 
        <strong>Equipment Id: <small>{ this.props.equipment.equipmentCode }</small></strong>
      }>
      {(() => {
        return <Form>
          <Grid fluid>
            <Row>
              <Col md={12}>
                <FormGroup controlId="make">
                  <ControlLabel>Make</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.make } updateState={ this.updateState } inputRef={ ref => { this.input = ref; }}/>                  
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="model">
                  <ControlLabel>Model</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.model } updateState={ this.updateState }/>                  
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="year" validationState={ this.state.yearError ? 'error' : null }>
                  <ControlLabel>Year</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.year } updateState={ this.updateState }/>                  
                  <HelpBlock>{ this.state.yearError }</HelpBlock>
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="size">
                  <ControlLabel>Size</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.size } updateState={ this.updateState }/>                  
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="type">
                  <ControlLabel>Type</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.type } updateState={ this.updateState }/>                  
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="licencePlate">
                  <ControlLabel>Licence Number</ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.licencePlate } updateState={ this.updateState }/>
                </FormGroup>
              </Col>
            </Row>
            <Row>
              <Col md={12}>
                <FormGroup controlId="serialNumber" validationState={ this.state.serialNumberError ? 'error' : null }>
                  <ControlLabel>Serial Number <sup>*</sup></ControlLabel>
                  <FormInputControl type="text" defaultValue={ this.state.serialNumber } updateState={ this.updateState } />
                  <HelpBlock>{ this.state.serialNumberError }</HelpBlock>
                </FormGroup>
              </Col>
            </Row>
          </Grid>
        </Form>;
      })()}
    </EditDialog>;
  },
});

function mapStateToProps(state) {
  return {
    equipment: state.models.equipment,
  };
}

export default connect(mapStateToProps)(EquipmentEditDialog);
