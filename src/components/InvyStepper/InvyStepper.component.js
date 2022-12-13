import React from 'react'
import PropTypes from 'prop-types'
import { makeStyles } from '@material-ui/core/styles'
import MobileStepper from '@material-ui/core/MobileStepper'

const useStyles = makeStyles({
  progress: {
    width: '100%'
  }
})

const InverLevyStepper = ({ step, steps }) => {
  const classes = useStyles()
  return <MobileStepper
    variant="progress"
    steps={steps}
    classes={{ progress: classes.progress }}
    position="static"
    style={{ backgroundColor: 'white', padding: 0 }}
    activeStep={step}
  />
}

export default InverLevyStepper

InverLevyStepper.propTypes = {
  step: PropTypes.number.isRequired,
  steps: PropTypes.number.isRequired
}
