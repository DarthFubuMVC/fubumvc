import React from 'react'
import {Col, Row} from 'react-bootstrap'
import Description from './description'

var ModelBinding = React.createClass({
    getInitialState(){
        return {
            loading: true
        }
    },

    componentDidMount(){
        // TODO -- add parameters into this someday
        FubuDiagnostics.get('ModelBinding:binding_all', {}, data => {
            this.setState({description: data, loading: false});
        });
    },

    render(){
        if (this.state.loading){
            return (<div>Loading...</div>);
        }

        return (
            <Row>
                <Col xs={3} md={2}>
                  Model Binding in <a href="https://github.com/DarthFubuMVC/fubucore">FubuCore</a> is generally done by invoking the <code>IObjectResolver</code> service.  Model binding in FubuMVC can be heavily customized and extended by plugging in additional implementations of these finer-grained services to handle types, handle properties, and converting values in new ways:
                  <br></br>
                  <ol>
                    <li>
                      <b>IModelBinder:  </b>The most coarse-grained level of customization.  An <code>IModelBinder</code> implementation "knows" how to create an object from the request data.  Most types will be handled by the StandardModelBinder that just calls a public constructor and delegates to <code>IPropertyBinder</code> services for each public setter.</li>
                    <li>
                      <b>IPropertyBinder:  </b>A model binding policy that can handle the model binding to one property at a time.  Most properties will be handled by the <code>ConversionPropertyBinder</code> that just finds the raw data from the request that matches the property name, converts or coerces the raw data to the property type, and sets the property value accordingly.  You might use custom <code>IPropertyBinder</code> services for information that isn't available in the http request.
                  </li>
                    <li>
                      <b>IConversionFamily:  </b>A fine grained service that "knows" how to coerce (or find) a raw string value into a requested type.
                    </li>
                  </ol>

                  <p>These services can be directly plugged in via your application's IoC container or through the <code>FubuRegisty.Model.******</code> methods in your FubuRegistry class.  Do keep in mind that ordering is important.  If two or more model binders / property binders / converter families could handle a scenario, the first one will always be used.
                </p>
                </Col>
                <Col xs={9} md={9}>
                    <Description {...this.state.description} />
                </Col>
            </Row>
        );
    }
});

module.exports = ModelBinding;
