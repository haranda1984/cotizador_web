/* eslint-disable react/prop-types */
import React, { useEffect, useState } from 'react'
import axios from 'axios'
import styled from 'styled-components'
import { navigate } from '@reach/router'

import TextField from '@material-ui/core/TextField'
import { PRIMARYCOLOR } from '../../constants/utils'
import { InverlevyButton } from '../../components/InvyButton/InvyButton.component'
import Snackbar from '@material-ui/core/Snackbar'
import MuiAlert from '@material-ui/lab/Alert'
import endpoints from '../../api-service/endpoints'
import Loader from 'react-loader-spinner'

const StyledDetailsWrapper = styled.div`
  display: flex;
  height: 100%;
  align-items:center;
  justify-content:center;
  flex-flow: column;
`

const StyledPaper = styled.div`
  background-color: white; 
  display: flex;
  flex-flow: column;
  align-items:center;
  justify-content:center;
  box-sizing: border-box;
  box-shadow: 0px 4px 4px rgba(0, 0, 0, 0.25);
  border-radius: 10px;
  padding: 2em;
  width:25%;
`

function Alert(props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />
}

const Details = (props) => {
  const [state, setstate] = useState({})
  const [loading, setloading] = useState(false)
  const [error, seterror] = useState({ message: '', display: false })

  useEffect(() => {
    if (props && props.location) {
      const data = { ...props.location.state }
      const body = {
        "project-id": data.product,
        "unit-id": data.unit.id,
        "trade-policy-id": data.tradepolicy.id,
        "date": data['initial-date'],
        "discount": data.discount,
        "price": data['final-price'],
        "deposit": data['deposit-amount'],
        "additions-payments": [...data['manual-additions'], ...data['additions-payments']],
        "last-payment": data['lastPayment-amount'],
        "currency": data.currency,
        "customer": {}
      }
      setstate({ ...body })
    }
  }, [props])

  const handleClick = async () => {
    setloading(true)
    try {
      // ToDo Make it required

      const response = await axios.post(`${endpoints.CALCULATOR}/summary`, state, { responseType: 'blob' })
      const blob = new Blob([response.data], { type: 'application/pdf' })

      const link = document.createElement('a')
      link.style = 'display: none'
      document.body.appendChild(link)

      const blobURL = URL.createObjectURL(blob)
      link.href = blobURL
      link.download = 'Cotizacion de Inmueble.pdf'
      link.click()
      URL.revokeObjectURL(blobURL)

      navigate('review')
    } catch (error) {
      seterror({ display: true, message: error })
      console.log(error)
    } finally {
      setloading(false)
    }
  }

  return <StyledDetailsWrapper>
    <Snackbar open={error.display} autoHideDuration={10000} onClose={() => { seterror({ message: '', display: false }) }}>
      <Alert onClose={() => { seterror({ message: '', display: false }) }} severity="error">
        {`Ocurri?? un error (${error.message}) al generar la corrida, por favor intente de nuevo. Si el error persiste contacte soporte soporte@rootinc.mx`}
      </Alert>
    </Snackbar>
    <StyledPaper>
      {loading === true ? <Loader type="ThreeDots" color={PRIMARYCOLOR} />
        : <> <h1 style={{ color: PRIMARYCOLOR }}>Inserta tus datos</h1>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, firstname: e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="Nombre" placeholder="Nombre" color="secondary" ></TextField>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, lastname: e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="Apellidos" placeholder="Apellidos" color="secondary" ></TextField>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, email: e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="Correo electr??nico" placeholder="Correo electr??nico" color="secondary" ></TextField>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, phone: e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="Tel??fono" placeholder="Tel??fono" color="secondary" ></TextField>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, address: e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="Direcci??n" placeholder="Direcci??n" color="secondary"></TextField>
          <TextField onChange={(e) => { setstate({ ...state, customer: { ...state.customer, 'tax-id': e.target.value } }) }} style={{ width: '80%' }} margin="normal" variant="outlined" label="RFC" placeholder="RFC" color="secondary"></TextField>
          <InverlevyButton onClick={handleClick} >Descargar Cotizaci??n</InverlevyButton></>
      }
    </StyledPaper>
  </StyledDetailsWrapper>
}

export default Details
