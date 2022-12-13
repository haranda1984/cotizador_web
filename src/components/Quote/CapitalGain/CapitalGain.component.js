import React, { useEffect, useState } from 'react'
import axios from 'axios'
import PropTypes from 'prop-types'
import Loader from 'react-loader-spinner'
import Snackbar from '@material-ui/core/Snackbar'
import MuiAlert from '@material-ui/lab/Alert'

import { PRIMARYCOLOR } from '../../../constants/utils'
import Wrapper from '../Wrapper/Wrapper.component'
import QuoteDetailsBox from '../QuoteDetailsBox'
import endpoints from '../../../api-service/endpoints'
import styled from 'styled-components'

function Alert(props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />
}

const CapitalGainWrapper = styled.div`
  display:grid;
  grid-template-columns: ${p => p.hasExtraOperation ? '1fr 1fr 1fr 1fr' : '2fr 1fr 1fr'};
  grid-template-areas: ${p => p.hasExtraOperation ? '". . . ." "start start final final"' : '". . ." ". final final"'};
  grid-gap: 1em;
  width:100%;
`

const parseErrorMessage = (error) => {
  var message = ''
  for (var key in error) {
    message = `${message} ${key} ${error[key].join(' ')}`
  }
  return message
}

const CapitalGain = ({ state, dispatch }) => {
  const [gain, setgain] = useState({})
  const [error, seterror] = useState({ message: '', display: false })

  const loadResult = async () => {
    try {
      const body = {
        "project-id": state.quote.product,
        "unit-id": state.quote.unit.id,
        "trade-policy-id": state.quote.tradepolicy.id,
        "date": state.quote['initial-date'],
        "discount": state.quote.discount,
        "price": state.quote['final-price'],
        "deposit": state.quote['deposit-amount'],
        "additions-payments": [...state.quote['manual-additions'], ...state.quote['additions-payments']],
        "last-payment": state.quote['lastPayment-amount'],
        "customer": {}
      }
      const response = await axios.post(endpoints.CALCULATOR, body)
      setgain(response.data)
      dispatch({ type: 'SET_HAS_ERROR', payload: false })
      dispatch({ type: 'SET_IS_CAPITALGAIN_LOADING', payload: false })
    } catch (error) {
      dispatch({ type: 'SET_HAS_ERROR', payload: true })
      if (error && error.response && error.response.status === 400) {
        seterror({ display: true, message: `${error.message} ${parseErrorMessage(error.response.data)}` })
      } else {
        seterror({ display: true, message: error.message })
      }
      console.log(error)
    } finally {
      dispatch({ type: 'SET_IS_CAPITALGAIN_LOADING', payload: false })
    }
  }

  useEffect(() => {
    loadResult()
  }, [])

  if (error.message) {
    return <Wrapper>
      <Snackbar open={error.display} autoHideDuration={6000} onClose={() => { seterror({ message: '', display: false }) }}>
        <Alert onClose={() => { seterror({ message: '', display: false }) }} severity="error">
          {`Ocurrió un error (${error.message}) al generar la corrida, por favor intente de nuevo. Si el error persiste contacte soporte soporte@rootinc.mx`}
        </Alert>
      </Snackbar>
      :(
    </Wrapper>
  }

  return <Wrapper>

    {state.isCapitalGainLoading === true ? <Loader type="ThreeDots" color={PRIMARYCOLOR} />
      : <CapitalGainWrapper hasExtraOperation={true}>
        <QuoteDetailsBox value={gain['rent-month']} title={'Renta mensual'} fontSize={22} />
        <QuoteDetailsBox value={gain['rent-year']} title={'Renta anual'} fontSize={22} />
        <QuoteDetailsBox value={`${(gain['cap-rate-Final'] * 100).toFixed(2)} %`} title={'Cap Rate Final'} fontSize={22} isText />
        <QuoteDetailsBox value={`${(gain['cap-rate-investment'] * 100).toFixed(2)} %`} title={'Cap Rate Actual'} fontSize={22} isText />
        <QuoteDetailsBox value={gain['investment']} gainPercentage={gain['roi']} title={'Inversión total'} subtitle={"Porcentaje de capitalización"} gridArea={'start'} active />
        <QuoteDetailsBox value={gain['minimum-expected-value']} title={'Valor minimo final'} gridArea="final" main />
      </CapitalGainWrapper>}
  </Wrapper>
}

export default CapitalGain

CapitalGain.propTypes = {
  state: PropTypes.object.isRequired,
  dispatch: PropTypes.func
}
